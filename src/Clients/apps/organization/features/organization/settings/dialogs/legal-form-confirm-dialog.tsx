import { LEGAL_FORM_LABELS, LegalForm } from "@workspace/types/organization";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@workspace/ui/components/alert-dialog";

interface LegalFormConfirmDialogProps {
  open: boolean;
  onClose: () => void;
  onConfirm: () => void;
  newLegalForm: LegalForm;
}

export function LegalFormConfirmDialog({
  open,
  onClose,
  onConfirm,
  newLegalForm,
}: Readonly<LegalFormConfirmDialogProps>) {
  return (
    <AlertDialog open={open} onOpenChange={(v) => !v && onClose()}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Сменить форму собственности?</AlertDialogTitle>
          <AlertDialogDescription>
            Вы меняете правовую форму на{" "}
            <strong>«{LEGAL_FORM_LABELS[newLegalForm]}»</strong>. Это затронет
            шаблоны договоров и формат отчётов. Существующие документы останутся
            в архиве, но новые будут формироваться по новой форме.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel onClick={onClose}>Отмена</AlertDialogCancel>
          <AlertDialogAction onClick={onConfirm}>Да, сменить</AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}

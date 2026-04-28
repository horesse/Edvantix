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

interface ResetConfirmDialogProps {
  open: boolean;
  onClose: () => void;
  onConfirm: () => void;
}

export function ResetConfirmDialog({
  open,
  onClose,
  onConfirm,
}: Readonly<ResetConfirmDialogProps>) {
  return (
    <AlertDialog open={open} onOpenChange={(v) => !v && onClose()}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Отменить изменения?</AlertDialogTitle>
          <AlertDialogDescription>
            Все несохранённые изменения будут потеряны. Это действие нельзя
            отменить.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel onClick={onClose}>
            Нет, продолжить
          </AlertDialogCancel>
          <AlertDialogAction
            className="bg-destructive hover:bg-destructive/90 text-white"
            onClick={onConfirm}
          >
            Да, отменить
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}

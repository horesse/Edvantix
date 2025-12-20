import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  GraduationCap,
  Users,
  BookOpen,
  BarChart3,
  CheckCircle2,
  Sparkles,
} from "lucide-react";

export default async function LandingPage() {
  // Это серверный компонент (SSR по умолчанию в Next.js App Router)
  // Здесь можно делать fetch данных на сервере

  return (
    <div className="min-h-screen bg-white">
      {/* Header */}
      <header className="border-b border-gray-200 sticky top-0 bg-white/80 backdrop-blur-md z-50">
        <div className="container mx-auto px-6 py-4">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-2">
              <div className="flex items-center justify-center w-10 h-10 bg-primary rounded-lg">
                <GraduationCap className="w-6 h-6 text-primary-foreground" />
              </div>
              <span className="text-xl font-bold">Edvantix</span>
            </div>
            <nav className="hidden md:flex items-center gap-8">
              <a href="#features" className="text-sm font-medium hover:text-primary transition-colors">
                Возможности
              </a>
              <a href="#benefits" className="text-sm font-medium hover:text-primary transition-colors">
                Преимущества
              </a>
              <a href="#pricing" className="text-sm font-medium hover:text-primary transition-colors">
                Цены
              </a>
            </nav>
            <div className="flex items-center gap-4">
              <Button variant="ghost">Войти</Button>
              <Button>Начать</Button>
            </div>
          </div>
        </div>
      </header>

      {/* Hero Section */}
      <section className="py-20 px-6 bg-gradient-to-br from-blue-50 via-white to-purple-50">
        <div className="container mx-auto max-w-6xl">
          <div className="text-center space-y-6">
            <div className="inline-flex items-center gap-2 px-4 py-2 bg-primary/10 rounded-full text-primary text-sm font-medium">
              <Sparkles className="w-4 h-4" />
              <span>Современная платформа для онлайн образования</span>
            </div>
            <h1 className="text-5xl md:text-6xl font-bold text-gray-900 max-w-4xl mx-auto">
              Управляйте вашей онлайн школой{" "}
              <span className="text-primary">эффективно</span>
            </h1>
            <p className="text-xl text-muted-foreground max-w-2xl mx-auto">
              Edvantix — это комплексная система управления онлайн школой.
              Создавайте курсы, управляйте студентами и отслеживайте их прогресс в одном месте.
            </p>
            <div className="flex items-center justify-center gap-4 pt-4">
              <Button size="lg" className="text-base">
                Попробовать бесплатно
              </Button>
              <Button size="lg" variant="outline" className="text-base">
                Посмотреть демо
              </Button>
            </div>
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section id="features" className="py-20 px-6">
        <div className="container mx-auto max-w-6xl">
          <div className="text-center space-y-4 mb-16">
            <h2 className="text-4xl font-bold text-gray-900">
              Все необходимое для вашей школы
            </h2>
            <p className="text-xl text-muted-foreground max-w-2xl mx-auto">
              Мощные инструменты для управления образовательным процессом
            </p>
          </div>

          <div className="grid gap-8 md:grid-cols-2 lg:grid-cols-3">
            <Card>
              <CardHeader>
                <div className="w-12 h-12 bg-primary/10 rounded-lg flex items-center justify-center mb-4">
                  <Users className="w-6 h-6 text-primary" />
                </div>
                <CardTitle>Управление студентами</CardTitle>
                <CardDescription>
                  Полный контроль над базой студентов, их прогрессом и активностью
                </CardDescription>
              </CardHeader>
            </Card>

            <Card>
              <CardHeader>
                <div className="w-12 h-12 bg-primary/10 rounded-lg flex items-center justify-center mb-4">
                  <BookOpen className="w-6 h-6 text-primary" />
                </div>
                <CardTitle>Создание курсов</CardTitle>
                <CardDescription>
                  Интуитивный конструктор курсов с поддержкой различных форматов контента
                </CardDescription>
              </CardHeader>
            </Card>

            <Card>
              <CardHeader>
                <div className="w-12 h-12 bg-primary/10 rounded-lg flex items-center justify-center mb-4">
                  <BarChart3 className="w-6 h-6 text-primary" />
                </div>
                <CardTitle>Аналитика</CardTitle>
                <CardDescription>
                  Детальная статистика и отчеты по всем аспектам обучения
                </CardDescription>
              </CardHeader>
            </Card>

            <Card>
              <CardHeader>
                <div className="w-12 h-12 bg-primary/10 rounded-lg flex items-center justify-center mb-4">
                  <GraduationCap className="w-6 h-6 text-primary" />
                </div>
                <CardTitle>Работа с преподавателями</CardTitle>
                <CardDescription>
                  Организация работы преподавательского состава и распределение нагрузки
                </CardDescription>
              </CardHeader>
            </Card>

            <Card>
              <CardHeader>
                <div className="w-12 h-12 bg-primary/10 rounded-lg flex items-center justify-center mb-4">
                  <CheckCircle2 className="w-6 h-6 text-primary" />
                </div>
                <CardTitle>Тестирование</CardTitle>
                <CardDescription>
                  Создавайте тесты и экзамены с автоматической проверкой
                </CardDescription>
              </CardHeader>
            </Card>

            <Card>
              <CardHeader>
                <div className="w-12 h-12 bg-primary/10 rounded-lg flex items-center justify-center mb-4">
                  <Sparkles className="w-6 h-6 text-primary" />
                </div>
                <CardTitle>Интеграции</CardTitle>
                <CardDescription>
                  Подключайте популярные сервисы для расширения возможностей
                </CardDescription>
              </CardHeader>
            </Card>
          </div>
        </div>
      </section>

      {/* Benefits Section */}
      <section id="benefits" className="py-20 px-6 bg-gray-50">
        <div className="container mx-auto max-w-6xl">
          <div className="grid gap-12 lg:grid-cols-2 items-center">
            <div className="space-y-6">
              <h2 className="text-4xl font-bold text-gray-900">
                Почему выбирают Edvantix?
              </h2>
              <div className="space-y-4">
                <div className="flex gap-4">
                  <CheckCircle2 className="w-6 h-6 text-primary flex-shrink-0 mt-1" />
                  <div>
                    <h3 className="font-semibold text-lg mb-1">Простота использования</h3>
                    <p className="text-muted-foreground">
                      Интуитивный интерфейс, не требующий технических знаний
                    </p>
                  </div>
                </div>
                <div className="flex gap-4">
                  <CheckCircle2 className="w-6 h-6 text-primary flex-shrink-0 mt-1" />
                  <div>
                    <h3 className="font-semibold text-lg mb-1">Безопасность данных</h3>
                    <p className="text-muted-foreground">
                      Надежная защита информации студентов и преподавателей
                    </p>
                  </div>
                </div>
                <div className="flex gap-4">
                  <CheckCircle2 className="w-6 h-6 text-primary flex-shrink-0 mt-1" />
                  <div>
                    <h3 className="font-semibold text-lg mb-1">Техподдержка 24/7</h3>
                    <p className="text-muted-foreground">
                      Всегда готовы помочь решить любые вопросы
                    </p>
                  </div>
                </div>
                <div className="flex gap-4">
                  <CheckCircle2 className="w-6 h-6 text-primary flex-shrink-0 mt-1" />
                  <div>
                    <h3 className="font-semibold text-lg mb-1">Регулярные обновления</h3>
                    <p className="text-muted-foreground">
                      Постоянное развитие и добавление новых функций
                    </p>
                  </div>
                </div>
              </div>
            </div>
            <div className="bg-gradient-to-br from-primary/20 to-purple-200 rounded-2xl aspect-square flex items-center justify-center">
              <GraduationCap className="w-48 h-48 text-primary/40" />
            </div>
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="py-20 px-6">
        <div className="container mx-auto max-w-4xl">
          <Card className="bg-gradient-to-br from-primary to-blue-600 text-white border-0">
            <CardContent className="p-12 text-center space-y-6">
              <h2 className="text-4xl font-bold">
                Готовы начать?
              </h2>
              <p className="text-xl text-blue-50">
                Присоединяйтесь к тысячам образовательных учреждений, которые уже используют Edvantix
              </p>
              <div className="flex items-center justify-center gap-4 pt-4">
                <Button size="lg" variant="secondary" className="text-base">
                  Начать бесплатный период
                </Button>
                <Button size="lg" variant="outline" className="text-base bg-transparent text-white border-white hover:bg-white/10">
                  Связаться с нами
                </Button>
              </div>
            </CardContent>
          </Card>
        </div>
      </section>

      {/* Footer */}
      <footer className="border-t border-gray-200 py-12 px-6">
        <div className="container mx-auto max-w-6xl">
          <div className="flex flex-col md:flex-row justify-between items-center gap-6">
            <div className="flex items-center gap-2">
              <div className="flex items-center justify-center w-8 h-8 bg-primary rounded-lg">
                <GraduationCap className="w-5 h-5 text-primary-foreground" />
              </div>
              <span className="font-bold">Edvantix</span>
            </div>
            <p className="text-sm text-muted-foreground">
              © 2024 Edvantix. Все права защищены.
            </p>
          </div>
        </div>
      </footer>
    </div>
  );
}

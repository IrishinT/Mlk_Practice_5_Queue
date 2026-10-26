using System;
using QueueLibrary.Dequeue;

namespace SlidingWindowProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            // Тестовый поток данных (например, показания сенсора или котировки)
            int[] stream = { 4, 2, 12, 3, -1, 5, 8, 1 };
            int windowSize = 3;

            Console.WriteLine("=== СКОЛЬЗЯЩЕЕ ОКНО (MIN / MAX ЧЕРЕЗ DEQUE) ===");
            Console.WriteLine($"Поток данных: [{string.Join(", ", stream)}]");
            Console.WriteLine($"Размер окна:  {windowSize}\n");

            // Запускаем поиск min и max в окне
            FindSlidingWindowMinMax(stream, windowSize);

            Console.WriteLine("\nНажмите любую клавишу для завершения...");
            Console.ReadKey();
        }

        public static void FindSlidingWindowMinMax(int[] data, int k)
        {
            if (data == null || data.Length == 0 || k <= 0)
                return;

            // Двусторонние очереди для хранения ИНДЕКСОВ
            // Можно использовать как ArrayDeque, так и LinkedDeque
            IDeque<int> maxDeque = new ArrayDeque<int>();
            IDeque<int> minDeque = new ArrayDeque<int>();

            for (int i = 0; i < data.Length; i++)
            {
                // 1. УДАЛЕНИЕ УСТАРЕВШИХ ИНДЕКСОВ (вышедших за левую границу окна)
                // Левая граница окна: i - k + 1. Все индексы <= i - k уже не в окне.
                if (maxDeque.HasNext() && maxDeque.PeekFirst() <= i - k)
                    maxDeque.PullFirst();

                if (minDeque.HasNext() && minDeque.PeekFirst() <= i - k)
                    minDeque.PullFirst();

                // 2. ПОДДЕРЖАНИЕ МОНОТОННОСТИ ДЛЯ MAX:
                // Выкидываем из хвоста всё, что меньше или равно новому элементу data[i]
                while (maxDeque.HasNext() && data[maxDeque.PeekLast()] <= data[i])
                {
                    maxDeque.PullLast();
                }
                maxDeque.OfferLast(i);

                // 3. ПОДДЕРЖАНИЕ МОНОТОННОСТИ ДЛЯ MIN:
                // Выкидываем из хвоста всё, что больше или равно новому элементу data[i]
                while (minDeque.HasNext() && data[minDeque.PeekLast()] >= data[i])
                {
                    minDeque.PullLast();
                }
                minDeque.OfferLast(i);

                // 4. ВЫВОД РЕЗУЛЬТАТА:
                // Окно начинает выводиться, когда накопилось k элементов (i >= k - 1)
                if (i >= k - 1)
                {
                    int maxIndex = maxDeque.PeekFirst();
                    int minIndex = minDeque.PeekFirst();

                    // Формируем строковое представление текущего окна
                    Console.Write($"Окно [индексы {i - k + 1}..{i}]: ");
                    PrintWindow(data, i - k + 1, i);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($" -> MAX: {data[maxIndex]} (на index {maxIndex})");

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($" | MIN: {data[minIndex]} (на index {minIndex})");
                    Console.ResetColor();
                }
            }
        }

        // Вспомогательный метод вывода текущих элементов окна без встроенных коллекций
        private static void PrintWindow(int[] data, int start, int end)
        {
            Console.Write("[");
            for (int j = start; j <= end; j++)
            {
                Console.Write(data[j] + (j == end ? "" : ", "));
            }
            Console.Write("]");
        }
    }
}
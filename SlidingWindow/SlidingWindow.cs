using System;
using QueueLibrary.Dequeue;

namespace SlidingWindowProgram
{
    class SlidingWindow
    {
        private static int[] stream = new int[8];
        private static int streamCount = 0;
        private const int WINDOW_SIZE = 3;

        // Деки для хранения индексов
        private static IDeque<int> maxDeque = new ArrayDeque<int>();
        private static IDeque<int> minDeque = new ArrayDeque<int>();

        static void Main(string[] args)
        {
            bool running = true;

            while (running)
            {
                // Выводим текущее состояние
                Console.WriteLine("\n========================================================");
                Console.WriteLine($" СКОЛЬЗЯЩЕЕ ОКНО: Размер K = {WINDOW_SIZE} | Чисел в потоке: {streamCount}");
                Console.WriteLine("========================================================");
                Console.WriteLine("1. Прогнать тестовые данные (поток чисел)");
                Console.WriteLine("2. Отправить число в поток вручную");
                Console.WriteLine("3. Сбросить поток");
                Console.WriteLine("4. Запустить тесты производительности");
                Console.WriteLine("0. Выход");
                Console.Write("\nВыберите пункт меню: ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        Reset();
                        int[] testData = { 4, 2, 12, 3, -1, 5, 8, 1 };
                        Console.WriteLine("Запуск симуляции потока...");
                        Console.WriteLine("Подача 8 чисел: [4, 2, 12, 3, -1, 5, 8, 1]...\n");

                        for (int i = 0; i < testData.Length; i++)
                        {
                            PushNumber(testData[i]);
                        }
                        break;

                    case "2":
                        Console.Write("Введите целое число: ");
                        string input = Console.ReadLine();

                        if (int.TryParse(input, out int num))
                        {
                            PushNumber(num);
                        }
                        else
                        {
                            Console.WriteLine("[!] Ошибка: введено не число");
                        }
                        break;

                    case "3":
                        Reset();
                        Console.WriteLine("[!] Поток очищен");
                        break;

                    case "4":
                        DequeBenchmark.RunPerformanceReport();
                        break;

                    case "0":
                        running = false;
                        Console.WriteLine("Выход из программы.");
                        break;

                    default:
                        Console.WriteLine("Неверный пункт, попробуйте еще раз.");
                        break;
                }
            }
        }

        // Обработка одного числа через скользящее окно
        private static void PushNumber(int val)
        {
            if (streamCount == stream.Length)
                Grow();

            int i = streamCount;
            stream[streamCount++] = val;

            // Удаление индексов, вышедших за окно слева
            if (maxDeque.HasNext() && maxDeque.PeekFirst() <= i - WINDOW_SIZE)
                maxDeque.PullFirst();

            if (minDeque.HasNext() && minDeque.PeekFirst() <= i - WINDOW_SIZE)
                minDeque.PullFirst();

            // Поддержание порядка в деках (срезаем меньшие/большие из хвоста)
            while (maxDeque.HasNext() && stream[maxDeque.PeekLast()] <= val)
                maxDeque.PullLast();
            maxDeque.OfferLast(i);

            while (minDeque.HasNext() && stream[minDeque.PeekLast()] >= val)
                minDeque.PullLast();
            minDeque.OfferLast(i);

            if (i < WINDOW_SIZE - 1)
            {
                Console.WriteLine($"[+] Число {val} записано. Окно {i + 1}/{WINDOW_SIZE}");
            }
            else
            {
                int maxIdx = maxDeque.PeekFirst();
                int minIdx = minDeque.PeekFirst();
                int startIdx = i - WINDOW_SIZE + 1;

                Console.WriteLine($"[+] Число {val,3} записано. Окно {GetWindowString(startIdx, i)} -> MAX: {stream[maxIdx],2} (idx={maxIdx}) | MIN: {stream[minIdx],2} (idx={minIdx})");
            }
        }

        private static string GetWindowString(int start, int end)
        {
            string res = "[";
            for (int j = start; j <= end; j++)
            {
                res += stream[j] + (j == end ? "" : ", ");
            }
            return res + "]";
        }

        private static void Reset()
        {
            streamCount = 0;
            maxDeque = new ArrayDeque<int>();
            minDeque = new ArrayDeque<int>();
        }

        private static void Grow()
        {
            int[] newArr = new int[stream.Length * 2];
            for (int i = 0; i < stream.Length; i++)
                newArr[i] = stream[i];
            stream = newArr;
        }
    }
}
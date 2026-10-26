using QueueLibrary.Dequeue;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlidingWindowProgram
{
    public class DequeBenchmark
    {
        public static void RunPerformanceReport()
        {
            Console.WriteLine("Запуск замеров производительности алгоритма скользящего окна...");

            int[] testArray = GenerateRandomArray(1000);

            // Разогрев
            for (int i = 0; i < 500; i++)
            {
                RunSlidingWindow(testArray, 50, new ArrayDeque<int>(), out _);
                RunSlidingWindow(testArray, 50, new LinkedDeque<int>(), out _);
            }

            var arrayDequeData = new List<(int N, double TimeMs, long Ops)>();
            var linkedDequeData = new List<(int N, double TimeMs, long Ops)>();

            Stopwatch sw = new Stopwatch();
            const int K = 100; // Размер окна

            // Основные замеры (размер потока от 10_000 до 200_000)
            for (int N = 10_000; N <= 200_000; N += 10_000)
            {
                int[] data = GenerateRandomArray(N);

                sw.Restart();
                RunSlidingWindow(data, K, new ArrayDeque<int>(K + 1), out long arrayOps);
                sw.Stop();
                arrayDequeData.Add((N, sw.Elapsed.TotalMilliseconds, arrayOps));

                sw.Restart();
                RunSlidingWindow(data, K, new LinkedDeque<int>(), out long linkedOps);
                sw.Stop();
                linkedDequeData.Add((N, sw.Elapsed.TotalMilliseconds, linkedOps));

                Console.WriteLine($"[+] N = {N,6} обработано");
            }

            using (var wb = new XLWorkbook())
            {
                AddSheet(wb, "ArrayDeque (Массив)", arrayDequeData);
                AddSheet(wb, "LinkedDeque (Список)", linkedDequeData);

                wb.SaveAs("DequePerformanceReport.xlsx");
                Console.WriteLine("\nОтчет сохранен в файл 'DequePerformanceReport.xlsx'!");
            }
        }

        // Алгоритм поиска Min/Max скользящего окна с подсчетом операций
        private static void RunSlidingWindow(int[] data, int k, IDeque<int> deque, out long ops)
        {
            ops = 0;

            for (int i = 0; i < data.Length; i++)
            {
                ops++; // шаг цикла

                // Удаление вышедших за окно
                if (deque.HasNext() && deque.PeekFirst() <= i - k)
                {
                    deque.PullFirst();
                    ops += 2;
                }

                // Срез элементов хуже текущего
                while (deque.HasNext() && data[deque.PeekLast()] <= data[i])
                {
                    deque.PullLast();
                    ops += 2;
                }

                deque.OfferLast(i);
                ops++;

                // Считывание результата
                if (i >= k - 1)
                {
                    int maxIdx = deque.PeekFirst();
                    ops++;
                }
            }
        }

        private static int[] GenerateRandomArray(int size)
        {
            var arr = new int[size];
            var rnd = new Random(42); // Фиксированный seed для одинаковых данных
            for (int i = 0; i < size; i++)
                arr[i] = rnd.Next(-1000, 1000);
            return arr;
        }

        private static void AddSheet(XLWorkbook wb, string name, List<(int N, double TimeMs, long Ops)> data)
        {
            var ws = wb.Worksheets.Add(name);
            ws.Cell(1, 1).Value = "Размер потока (N)";
            ws.Cell(1, 2).Value = "Время (мс)";
            ws.Cell(1, 3).Value = "Количество операций";
            ws.Row(1).Style.Font.Bold = true;

            for (int i = 0; i < data.Count; i++)
            {
                ws.Cell(i + 2, 1).Value = data[i].N;
                ws.Cell(i + 2, 2).Value = data[i].TimeMs;
                ws.Cell(i + 2, 3).Value = data[i].Ops;
            }

            ws.Columns().AdjustToContents();
        }
    }
}

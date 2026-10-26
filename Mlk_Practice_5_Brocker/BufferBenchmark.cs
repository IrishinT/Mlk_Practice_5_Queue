using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClosedXML.Excel;
using QueueLibrary.Queue;

namespace BrockerProgram
{
    public class BufferBenchmark
    {
        public static void RunPerformanceReport()
        {
            Console.WriteLine("Запуск замеров производительности буфера...");

            // Разогрев
            for (int i = 0; i < 1000; i++)
            {
                TestOfferWithOverwrite(100, 10, out _);
                TestPull(100, out _);
                TestStreamCycle(100, 10, out _);
            }

            var offerData = new List<(int N, double TimeMs, long Ops)>();
            var pullData = new List<(int N, double TimeMs, long Ops)>();
            var streamData = new List<(int N, double TimeMs, long Ops)>();

            Stopwatch sw = new Stopwatch();

            // Основные замеры по шагам N
            for (int N = 10_000; N <= 200_000; N += 10_000)
            {
                // Запись в буфер с постоянным вытеснением (буфер емкостью 1000)
                sw.Restart();
                TestOfferWithOverwrite(N, 1000, out long offerOps);
                sw.Stop();
                offerData.Add((N, sw.Elapsed.TotalMilliseconds, offerOps));

                // Вычитка N элементов из буфера
                sw.Restart();
                TestPull(N, out long pullOps);
                sw.Stop();
                pullData.Add((N, sw.Elapsed.TotalMilliseconds, pullOps));

                // Реалистичный поток (источник пишет 3, обработчик читает 1)
                sw.Restart();
                TestStreamCycle(N, 1000, out long streamOps);
                sw.Stop();
                streamData.Add((N, sw.Elapsed.TotalMilliseconds, streamOps));

                Console.WriteLine($"[+] N = {N,6} обработано");
            }

            // 3. Сохранение в Excel
            using (var wb = new XLWorkbook())
            {
                AddSheet(wb, "Запись с вытеснением", offerData);
                AddSheet(wb, "Вычитка (Pull)", pullData);
                AddSheet(wb, "Смешанный поток", streamData);

                wb.SaveAs("BufferPerformanceReport.xlsx");
                Console.WriteLine("\nОтчет сохранен в файл 'BufferPerformanceReport.xlsx'!");
            }
        }

        private static void TestOfferWithOverwrite(int N, int capacity, out long ops)
        {
            ops = 0;
            var buffer = new OverwritingCircularQueue<int>(capacity);
            ops++;

            for (int i = 0; i < N; i++)
            {
                buffer.Offer(i);
                ops += 2; // проверка переполнения + вставка в кольцо
            }
        }

        private static void TestPull(int N, out long ops)
        {
            ops = 0;
            var buffer = new CircularQueue<int>(N);
            for (int i = 0; i < N; i++) buffer.Offer(i);

            for (int i = 0; i < N; i++)
            {
                buffer.Pull();
                ops += 2; // чтение + сдвиг указателя
            }
        }

        private static void TestStreamCycle(int N, int capacity, out long ops)
        {
            ops = 0;
            var buffer = new OverwritingCircularQueue<int>(capacity);

            for (int i = 0; i < N; i++)
            {
                // 3 записи
                buffer.Offer(i);
                buffer.Offer(i);
                buffer.Offer(i);
                ops += 6;

                // 1 чтение
                if (buffer.HasNext())
                {
                    buffer.Pull();
                    ops += 2;
                }
            }
        }

        private static void AddSheet(XLWorkbook wb, string name, List<(int N, double TimeMs, long Ops)> data)
        {
            var ws = wb.Worksheets.Add(name);
            ws.Cell(1, 1).Value = "Размер (N)";
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
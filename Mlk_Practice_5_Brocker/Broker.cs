using System;
using QueueLibrary.Queue;

namespace BrockerProgram
{
    // Пакет данных для обмена между двумя системами
    public class DataPacket
    {
        public int Id { get; set; }
        public string Data { get; set; }

        public DataPacket(int id, string data)
        {
            Id = id;
            Data = data;
        }

        public override string ToString() => $"[Пакет #{Id} | Данные: {Data}]";
    }

    class Broker
    {
        static void Main(string[] args)
        {
            // Создание перезаписываемого буффера на 4 элемента
            const int BUFFER_CAPACITY = 4;
            var buffer = new OverwritingCircularQueue<DataPacket>(BUFFER_CAPACITY);

            int packetCounter = 1;
            bool running = true;

            while (running)
            {
                // Выводим текущее состояние буфера
                Console.WriteLine("\n========================================================");
                Console.WriteLine($" БУФЕР: Заполнено {buffer.GetCount()} из {buffer.GetCapacity()} | Вытеснено (потеряно): {buffer.DroppedCount}");
                Console.WriteLine("========================================================");
                Console.WriteLine("1. Прогнать тестовые данные (быстрый поток от датчика)");
                Console.WriteLine("2. Отправить пакет вручную");
                Console.WriteLine("3. Принять (обработать) пакет");
                Console.WriteLine("0. Выход");
                Console.Write("\nВыберите пункт меню: ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Запуск симуляции потока...");
                        Console.WriteLine("Датчик отправляет 6 пакетов данных...");

                        for (int i = 0; i < 6; i++)
                        {
                            int dropsBefore = buffer.DroppedCount;
                            var packet = new DataPacket(packetCounter++, $"Замер темп. {60 + i}F");

                            buffer.Offer(packet);

                            if (buffer.DroppedCount > dropsBefore)
                            {
                                Console.WriteLine($"[+!] {packet} записан, но буфер УЖЕ ПЕРЕПОЛНЕН, старый пакет вытеснен");
                            }
                            else
                            {
                                Console.WriteLine($"[+] {packet} успешно записан в буфер");
                            }
                        }
                        break;

                    case "2":
                        // Ручная отправка пакета
                        Console.Write("Введите полезную нагрузку (или нажмите Enter для генерации): ");
                        string input = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(input))
                            input = $"OPS #{packetCounter}";

                        int prevDrops = buffer.DroppedCount;
                        var manualPacket = new DataPacket(packetCounter++, input);

                        buffer.Offer(manualPacket);

                        if (buffer.DroppedCount > prevDrops)
                        {
                            Console.WriteLine($"[+!] Буфер был полон! Старый пакет удален ради {manualPacket}");
                        }
                        else
                        {
                            Console.WriteLine($"[+] {manualPacket} добавлен в буфер.");
                        }
                        break;

                    case "3":
                        // Принятие/обработка пакета
                        if (buffer.HasNext())
                        {
                            var processed = buffer.Pull();
                            Console.WriteLine($"<- обработан {processed}");
                        }
                        else
                        {
                            Console.WriteLine("[!] Буфер пуст");
                        }
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
    }
}
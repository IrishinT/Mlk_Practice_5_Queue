// Упражнение
// Реализовать дек как класс DEQ (методы):
// DeqWriteRight(key) - включение элемента справа
// DeqWriteLeft(key) - включение элемента слева
// <type_inf> DeqReadRight(DEQ R) - искл. эл. справа
// <type_inf> DeqReadLeft(DEQ L) - искл. эл. слева, DeqClear(),
// <целое положительное> DeqSize() - размер очереди
//
// Реализовать классы:
// - Классической очереди, циклической очереди, очереди с приоритетным включением и выключением
// Задачи на очереди
// - Разработать симулятор системы, которая получает потоковые данные (напр, брокер сообщений RabbitMQ, Redis)
// - Система должна показывать как забрать данные и как получить данные (с буффером)
// Реализовать для этого класс DEQ
// Используя скользящее окно найти максимум и минимум - и используя deq записать индексы

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace QueueLibrary.Queue
{
    public interface IQueue<T>
    {

        T Pull();

        T Peek();

        void Offer(T element);

        bool HasNext();

        bool IsEmpty();

    }
}

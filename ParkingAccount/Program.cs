using System.Text.RegularExpressions;

namespace ParkingAccount
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Парковочные места
            Dictionary<int, ParkingLotState> parkingLots = new Dictionary<int, ParkingLotState>();
            int parkingCapacity = 20;
            for (int i = 1; i <= parkingCapacity; i++)
            {
                parkingLots.Add(i, new ParkingLotState());
            }

            // Очередь авто, ожидающих парковки
            Queue<Car> waitingCars = new Queue<Car>();

            // Список припаркованных авто
            List<Car> cars = new List<Car>();
            
            while (true)
            {
                bool exitState = false;

                ParkingServices.EntranceInfo(parkingLots);

                var task = Console.ReadKey(true);

                switch (task.Key)
                {
                    // ДОБАВИТЬ АВТО НА ПАРКОВКУ
                    case ConsoleKey.D1:
                        Console.WriteLine();
                        Console.Write("Введите марку авто: ");
                        string carBrand = Console.ReadLine().ToUpper();
                        if (String.IsNullOrEmpty(carBrand))
                        {
                            ParkingServices.ErrorMessage("Вы ничего не ввели!");
                            Console.ReadKey(true);
                            break;
                        }

                        Console.Write("Введите номер авто в виде 'X000XX': ");
                        string carNumber = Console.ReadLine().ToUpper();
                        if (!Regex.IsMatch(carNumber, ParkingServices.patternNumber))
                        {
                            ParkingServices.ErrorMessage("Введённый номер не соответствует указанному формату!");
                            Console.ReadKey(true);
                            break;
                        }

                        // Проверка свободных парковочных мест. Если таких нет, то добавляем авто в очередь
                        if (ParkingServices.AnyFreeParkingLotsCheck(parkingLots) == 0)
                        {
                            waitingCars.Enqueue(new Car(carBrand, carNumber));
                            ParkingServices.ResultMessage("Свободных мест нет! Автомобиль был добавлен в очередь.");
                            Console.ReadKey(true);
                            break;
                        }
                        Console.Write("Выберите свободное место: ");
                        ParkingServices.ShowAllParkingLots(parkingLots);

                        var parkingLot = ParkingServices.GetUserInput();
                        if (parkingLot == 0) break;

                        if (parkingLot <= parkingCapacity && parkingLot > 0 && parkingLots[parkingLot] == ParkingLotState.Free)
                        {
                            ParkingServices.ParkTheCar(parkingLots, parkingLot, cars, carBrand, carNumber);
                        }
                        else
                        {
                            ParkingServices.ErrorMessage("Выбранный номер парковки недоступен!!!");
                            Console.ReadKey(true);
                            break;
                        }

                        ParkingServices.ResultMessage($"Авто марки '{carBrand}' с номером '{carNumber}' припаркован на {parkingLot} место.");
                        Console.ReadKey(true);
                        break;

                    // ПРОСМОТРЕТЬ СПИСОК ПРИПАРКОВАННЫХ АВТО
                    case ConsoleKey.D2:
                        if (ParkingServices.IsEmptyCheck<Car>(cars, "На данный момент нет припаркованных авто!") == 0) break;
                        Console.WriteLine();
                        Console.WriteLine("\tСписок припаркованных авто:");
                        ParkingServices.ShowAllParkedCars(cars);
                        ParkingServices.HelpMessage("---Для продолжения нажмите любую клавишу---");
                        Console.ReadKey(true);
                        break;

                    // ПРОСМОТРЕТЬ СПИСОК АВТО В ОЧЕРЕДИ
                    case ConsoleKey.D3:
                        if (ParkingServices.IsEmptyCheck<Car>(waitingCars, "На данный момент очередь пуста!") == 0) break;
                        Console.WriteLine();
                        Console.WriteLine("\tСписок авто в очереди:");
                        ParkingServices.ShowAllParkedCars(waitingCars);
                        ParkingServices.HelpMessage("---Для продолжения нажмите любую клавишу---");
                        Console.ReadKey(true);
                        break;

                    // УДАЛИТЬ АВТО С ПАРКОВКИ
                    case ConsoleKey.D4:
                        if (ParkingServices.IsEmptyCheck<Car>(cars, "На данный момент нет припаркованных авто!") == 0) break;
                        Console.WriteLine();
                        Console.WriteLine("\tВыберите авто для завершения парковки:");
                        ParkingServices.ShowAllParkedCars(cars);
                        ParkingServices.HelpMessage("---Введите номер авто из списка---");
                        var chosenCar = ParkingServices.GetUserInput();
                        if (chosenCar == 0) break;

                        if (chosenCar <= cars.Count && chosenCar > 0)
                        {
                            Car deletedCar = cars[chosenCar - 1];
                            parkingLots[deletedCar.parkingLot] = ParkingLotState.Free;
                            // Расчёт суммы оплаты за парковку
                            double paymentAmount = ParkingServices.ParkingPayment(deletedCar);
                            // Удаление авто из списка припаркованных
                            cars.RemoveAt(chosenCar - 1);
                            ParkingServices.FinishedCarMessage(deletedCar, paymentAmount);

                            // Запись сеанса парковки в журнал
                            using (StreamWriter writer = new StreamWriter("Журнал парковок.txt", true))
                            {
                                await writer.WriteLineAsync(deletedCar.TotalInfoCar(paymentAmount));
                            }
                            Console.ReadKey(true);

                            // Добавление авто из очереди на парковку, если такое есть
                            if (waitingCars.Count() != 0)
                            {
                                Car carInQueue = waitingCars.Dequeue();
                                carInQueue.parkingLot = deletedCar.parkingLot;
                                ParkingServices.ParkTheCar(parkingLots, cars, carInQueue);
                                ParkingServices.ResultMessage($"Авто марки '{carInQueue.carBrand}' с номером '{carInQueue.carNumber}' припаркован на {carInQueue.parkingLot} место.");
                                Console.ReadKey(true);
                            }
                        }
                        else
                        {
                            ParkingServices.ErrorMessage("Введённого номера нет в списке!");
                            Console.ReadKey(true);
                            break;
                        }
                        break;

                    // ЗАВЕРШИТЬ СЕАНС
                    case ConsoleKey.D0:
                        exitState = true;
                        break;
                    default:
                        Console.WriteLine();
                        ParkingServices.ErrorMessage("Введена некорректная команда!");
                        Console.ReadKey(true);
                        break;
                }
                if (exitState)
                {
                    break;
                }
            }
        }  
    }
    
}

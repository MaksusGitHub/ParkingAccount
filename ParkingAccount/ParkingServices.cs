using System.Collections;

// Состояние парковочного места
enum ParkingLotState
{
    Free,
    Busy
}

namespace ParkingAccount
{
    internal static class ParkingServices
    {
        // Регулярное выражение для номера машины
        internal const string patternNumber = @"^[А-Я]{1}\d{3}[А-Я]{2}$";

        // Вывод всех свободных парковочных мест
        internal static void ShowAllParkingLots(Dictionary<int, ParkingLotState> parkingLots)
        {
            foreach (var item in parkingLots)
            {
                if (item.Value == ParkingLotState.Free)
                {
                    Console.Write($"{item.Key} ");
                }
            }
            Console.WriteLine();
        }

        // Вывод всех припаркованных/ожидающихВочереди авто
        internal static void ShowAllParkedCars(IEnumerable allCars)
        {
            Console.WriteLine();
            int i = 0;
            foreach (var item in allCars)
            {
                i++;
                Console.WriteLine($"{i}. {item}");
            }
        }

        // Проверка свободных парковочных мест
        internal static int AnyFreeParkingLotsCheck(Dictionary<int, ParkingLotState> parkingLots)
        {
            int amountFreeParkingLots = 0;
            foreach (var item in parkingLots)
            {
                if (item.Value == ParkingLotState.Free)
                {
                    amountFreeParkingLots++;
                }
            }
            return amountFreeParkingLots;
        }

        // Получение и проверка введённого пользователем значения на цифровое
        internal static int GetUserInput()
        {
            var UserInput = Console.ReadKey(true);
            bool UserInputCheck = int.TryParse(UserInput.KeyChar.ToString(), out int resultInput);
            if (!UserInputCheck)
            {
                ParkingServices.ErrorMessage("Введено неверное значение!");
                Console.ReadKey();
                return 0;
            }
            else
            {
                return resultInput;
            }
        }

        // Припарковать авто
        internal static void ParkTheCar(Dictionary<int, ParkingLotState> parkingLots, int parkingLot, List<Car> cars, string carBrand, string carNumber)
        {
            parkingLots[parkingLot] = ParkingLotState.Busy;
            DateTime entryTime = DateTime.Now;
            cars.Add(new Car(carBrand, carNumber, parkingLot, entryTime));
        }

        // Припарковать авто из очереди
        internal static void ParkTheCar(Dictionary<int, ParkingLotState> parkingLots, List<Car> cars, Car car )
        {
            parkingLots[car.parkingLot] = ParkingLotState.Busy;
            car.entryTime = DateTime.Now;
            cars.Add(car);
        }

        // Расчёт итоговой суммы к оплате за парковку
        internal static double ParkingPayment(Car car)
        {
            double costPerMinute = 2;
            car.departureTime = DateTime.Now;
            car.parkingTime = car.departureTime.Subtract(car.entryTime);
            return car.parkingTime.Minutes * costPerMinute;
        }

        // Вывод ошибки
        internal static void ErrorMessage(string message)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.WriteLine("\tПовторите попытку");
            HelpMessage("---Для продолжения нажмите любую клавишу---");
        }

        // Вывод результата какого-либо действия
        internal static void ResultMessage(string message)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            HelpMessage("---Для продолжения нажмите любую клавишу---");
        }

        // Сообщение - подсказка
        internal static void HelpMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        // Стартовое меню выбора действия
        internal static void EntranceInfo(Dictionary<int, ParkingLotState> parkingLots)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\tРегистрация автомобилей на парковке");
            Console.WriteLine();
            Console.ResetColor();
            Console.WriteLine($"Количество свободных парковочных мест - {parkingLots.Where(p => p.Value == ParkingLotState.Free).Count()}");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Выберите необходимое действие:");
            Console.WriteLine("1 - Добавить новое авто на парковку");
            Console.WriteLine("2 - Просмотреть список припаркованных авто");
            Console.WriteLine("3 - Просмотреть список авто в очереди");
            Console.WriteLine("4 - Удалить авто с парковки");
            Console.WriteLine("0 - Завершить сеанс");
            Console.ResetColor();
        }

        // Вывод информации по авто, завершимшему сеанс парковки
        internal static void FinishedCarMessage(Car car, double paymentAmount)
        {
            Console.WriteLine();
            Console.WriteLine($"Автомобиль ({car}) завершил парковку.");
            Console.WriteLine($"Время выезда: {car.departureTime}. Общее время парковки: {car.parkingTime.Minutes} мин.");
            ResultMessage($"К оплате {paymentAmount} руб.");
        }

        // Проверка последовательности на пустоту
        internal static int IsEmptyCheck<T>(IEnumerable<T> allCars, string message)
        {
            if (allCars.IsNullOrEmpty())
            {
                ErrorMessage(message);
                Console.ReadKey(true);
                return 0;
            }
            else
            {
                return 1;
            }
        }

        // Метод расширения проверки пустоты последовательности
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> items)
        {
            return items == null || !items.Any();
        }
    }
}

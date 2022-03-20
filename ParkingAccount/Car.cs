namespace ParkingAccount
{
    internal class Car
    {
        // Марка авто
        private string _carBrand;
        public string carBrand { get { return _carBrand; } }
        // Номер авто
        private string _carNumber;
        public string carNumber { get { return _carNumber; } }
        // Место парковки авто
        private int _parkingLot;
        public int parkingLot { get { return _parkingLot; } set { _parkingLot = value; } }

        // Время въезда авто на парковку
        private DateTime _entryTime;
        public DateTime entryTime { get { return _entryTime; } set { _entryTime = value; } }
        // Время выезда авто с парковки
        public DateTime departureTime { get; set; }
        // Время пребывания авто на парковке
        public TimeSpan parkingTime { get; set; }

        public Car(string carBrand, string carNumber, int parkingLot, DateTime entryTime)
        {
            _carBrand = carBrand;
            _carNumber = carNumber;
            _parkingLot = parkingLot;
            _entryTime = entryTime;
        }

        public Car(string carBrand, string carNumber)
        {
            _carBrand = carBrand;
            _carNumber = carNumber;
        }

        // Весь сеанс парковки авто
        public string TotalInfoCar(double paymentAmount)
        {
            return $"Марка: {_carBrand}; Номер: {_carNumber}; Парковочное место: {_parkingLot}; Время заезда: {_entryTime}; " +
             $"Время выезда: {departureTime}; Общее время парковки: {parkingTime.Minutes} мин.; Сумма оплаты: {paymentAmount} руб.";
        }

        public override string ToString()
        {
            return $"Марка: {_carBrand}; Номер: {_carNumber}; Парковочное место: {_parkingLot}; Время заезда: {_entryTime}";
        }
    }
}

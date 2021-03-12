using PatrolePumpSceduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Vehicles;

namespace PatroleStation
{
    public class Pump
    {
        public string Name { get; } //name of the pump
        public bool isAvailable { get; private set; } //is pump available or availed by vehicle
        public int NumberOfVehicleServiced { get; private set; } //total number of vehicle served by this pump
        public double Dispensed { get; private set; } //total number of liter dispensed
        private Random random; //random generator object variable
        private Timer dispensing_timer; //timer object variable
        private float Dispensing { get; } //dispensing rate (liters/sec)
        public string CurrentVehicle { get; set; } //current active vehicle 

        public Pump(string Name, int seed = 232323 ,float Dispensing = 1.5f) //constructor
        {
            random = new Random(seed); //init random generator
            isAvailable = true; //make pump available by default
            this.Name = Name; //add name
            NumberOfVehicleServiced = 0;
            Dispensed = 0; 
            this.Dispensing = Dispensing;
            CurrentVehicle = "(free)"; //adding current vehicle as (free) by default
        }

        //serve the vehicle and fill the petrole
        public void Serve(Vehicle v, Pump p1 = null, Pump p2 = null)
        {
            if(p1 != null) //if p1 is passed means that it should be blocked as per advance queing system
            {
                p1.isAvailable = false; //make pump unavailable
                p1.CurrentVehicle = "(blocked)"; //add blocked status
            }

            if (p2 != null)  //if p2 is passed means that it should be blocked as per advance queing system
            {
                p2.isAvailable = false;
                p2.CurrentVehicle = "(blocked)";
            }

            CurrentVehicle = string.Format("{0}#{1}", v.Type, v.Name); //add vehicle name as current vehicle
            isAvailable = false; //lock the pump
            double _dispensing_time_available = random.Next(16000, 20000); //calculate random dispensing time
            double _dispensed_liter = (_dispensing_time_available / 1000) * Dispensing; //calculate dispensing liters by formula

            if(v.Type.GetHashCode() < (v.Tank + _dispensed_liter)) //if tank is overflow
            {
                _dispensed_liter = v.Type.GetHashCode() - v.Tank; //get liters as per tank capacity
                v.Tank = _dispensed_liter;
            }
            else
            {
                v.Tank += _dispensed_liter; //add fuel to vehicle
            }
            
            NumberOfVehicleServiced++; //increment vehicle count
            dispensing_timer = new Timer(_dispensing_time_available); //add the timer for dispensing
            dispensing_timer.Elapsed += (sender, e) => { Dispensing_timer_Elapsed(sender, e, _dispensed_liter,p1,p2); }; //add the event to the timer
            dispensing_timer.Start(); //start the timerr
            IO.update(); //update the output
        }

        //dispensing time event
        private void Dispensing_timer_Elapsed(object sender, ElapsedEventArgs e, double _d, Pump p1 = null, Pump p2 = null)
        {
            Dispensed += _d; //incrment the dispensed liters
            isAvailable = true; //unlock pump
            CurrentVehicle = "(free)"; //update status
            dispensing_timer.Dispose(); //delete the timer

            //update locked pump as per advance queue
            if (p1 != null)
            {
                p1.isAvailable = true;
                p1.CurrentVehicle = "(free)";
            }

            //update locked pump as per advance queue
            if (p2 != null)
            {
                p2.isAvailable = true;
                p2.CurrentVehicle = "(free)";
            }

            //update output
            IO.update();
        }
        
        //function to get the details
        public override string ToString()
        {
            return String.Format("{0}-{1,-10}\tServiced: {2}, Dispensed: {3} l", Name, CurrentVehicle ,NumberOfVehicleServiced, Dispensed);
        }
    }

    public class Station
    {
        //list that contains all pumps
        List<Pump> pumps;
        public float AmountPerLiter { get; }
        public float Commission { get; }
        private Random random;

        //constructor
        public Station(float AmountPerLiter = 1, float Commission = 1)
        {
            this.AmountPerLiter = AmountPerLiter;
            this.Commission = Commission;
            random = new Random();
            pumps = new List<Pump>(6); //init the list

            //add 9 petrole pump to the list
            for(int i = 0; i < 9; i++)
            {
                pumps.Add(new Pump(String.Format("{0}#{1}","Pump",i+1), random.Next()));
            }
        }

        public void serveVehicle(Vehicle v, int pumpNumber)
        {
            int temp = pumpNumber - 1; //normalize the pump number
            if (v == null) //if no vehicle found skip the function
                return;

            if (temp < 0 || temp > 9) //if wrong number passed skip the function
                return;

            if (!pumps[temp].isAvailable) //if pump is unavailable skip the function
                return;

            int mod = temp % 3; //mod to calculate the advance queing system
            if(mod == 0) //if mod is 0 then it means block next two pumps i.e. temp+1 and temp+2
                pumps[temp].Serve(v,pumps[temp+1].isAvailable? pumps[temp + 1]:null, pumps[temp+2].isAvailable? pumps[temp + 2]:null);
            else if(mod == 1) //if mod is 1 then it means block next pump i.e. temp+1
                pumps[temp].Serve(v, pumps[temp + 1].isAvailable? pumps[temp + 1]:null);
            else //if mod is 2 then it means no pump is blocked
                pumps[temp].Serve(v);
        }

        public Pump[] getPumps()
        {
            return pumps.ToArray(); //convert pump to array and return the array of all pumps
        }

        //get all pumps details
        public override string ToString()
        {
            double totalFueled = 0;

            foreach (var pump in pumps) //calculate total fuel dispensed
            {
                totalFueled += pump.Dispensed;
            }

            return String.Format("Dispensed: {0} l, Ammount: {1}£, Commission: {2}£", totalFueled, totalFueled * AmountPerLiter, (totalFueled * AmountPerLiter * (Commission / 100)));
        }
    }
}

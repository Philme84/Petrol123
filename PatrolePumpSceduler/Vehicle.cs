using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using PatrolePumpSceduler;

namespace Vehicles
{
    //vehicle type and their capacity
    public enum Vehicle_Type { Cars = 50, Vans = 80, HGV = 150 }

    public class Vehicle
    {
        public string Name { get; }
        public double Tank { get; set; } //tank capacity
        public Vehicle_Type Type { get; } //to store vehcile type
        Random random;
        public Vehicle()
        {
            random = new Random();
            Name = string.Format("{0:0000}", random.Next(0000, 9999)); //generate randome name
            Array t = Enum.GetValues(typeof(Vehicle_Type)); //get all vehicle pre-defined types
            Type = (Vehicle_Type)t.GetValue(random.Next(0, t.Length)); //select type random
            Tank = random.Next(0, Type.GetHashCode() / 4); //select ranodm fuel in tank i.e. 1/4 of capacity
        }

        //get vehicle details
        public override string ToString()
        {
            return Type.ToString() + "#" + Name + "-" + Tank.ToString() + " Liters";
        }
    }

    public class VehicleContainer
    {
        //Constants
        private const int MAX_QUEUE = 5; //init max queue size
        //Variables
        private List<Vehicle> vehicles; //vehicle container
        private List<Timer> exp_timers; //each vehicle timer container
        private Timer createTimer; //timer to create the vehicle
        private Random random;
        public int TotalVehicle { get; private set; }
        public int TotalExpired { get; private set; }

        public VehicleContainer()
        {
            TotalVehicle = 0;
            TotalExpired = 0;
            random = new Random();
            vehicles = new List<Vehicle>(MAX_QUEUE); //init total vehicle list
            exp_timers = new List<Timer>(MAX_QUEUE); //init total timer list for vehicles
            createTimer = new Timer(random.Next(1500,2200)); //create expiry timer for vehicle creation
            createTimer.Elapsed += CreateTimer_Elapsed; //attach event
            createTimer.Start(); //start the timer
        }

        private void CreateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if(vehicles.Count >= MAX_QUEUE) //if max count reached skip
            {
                return;
            }

            createTimer.Interval = random.Next(1500, 2200); //make interval random again
            Vehicle v = new Vehicle(); //create new vehicle
            Timer t = new Timer(random.Next(2000, 3000)); //create new timer
            t.Elapsed += (object _sender, ElapsedEventArgs _e) => { expEventElapsed((Timer)_sender, _e, v); }; //attach event to expiry vehicle timer
            t.Start(); //start the timer
            exp_timers.Add(t); //add the expiry timer to container
            vehicles.Add(v); //add the vehicle to container
            TotalVehicle++; //increment total vehicle
            IO.update(); //update the output
        }

        private void expEventElapsed(Timer sender, ElapsedEventArgs e, Vehicle v)
        {
            if (!vehicles.Remove(v)) //if vehicle is already removed then skip the rset function
            {
                sender.Dispose(); //dispose the timer
                exp_timers.Remove(sender); //remove the expiry timer
                return;
            }    
            TotalExpired++; //increment the total expire vehicle
            IO.update(); //update the output
        }

        //get vehicle to be server
        public Vehicle getVehicleToServed()
        {
            if (vehicles.Count <= 0) //if no vehicle found
                return null; //return null
            Vehicle v = vehicles[0]; //else draw a first vehicle
            vehicles.RemoveAt(0); //remove that vehicle from the list
            return v; //return the vehicle
        }

        public Vehicle[] getVehicles()
        {
            return vehicles.ToArray(); //get all vehicles as array
        }

        //get all vehicles data
        public override string ToString()
        {
            return String.Format("Total: {0}, Fullfiled: {1}, Unfulfilled: {2}", TotalVehicle - vehicles.Count, TotalVehicle-(TotalExpired+vehicles.Count), TotalExpired);
        }
    }
}

using PatroleStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vehicles;

namespace PatrolePumpSceduler
{
    public class IO
    {
        private static VehicleContainer vc; //vehicle Container object
        private static Station station; //Station object
        private static bool updating; //lock to check the console update process is started or not, if started then lock it else update the console
        public static void init(VehicleContainer _vc, Station _station) //initialize the class
        {
            vc = _vc;
            station = _station;
            updating = false;
        }

        //get input from the console and return the int that the number is pressed
        public static int getInput() 
        {
            if (int.TryParse(Console.ReadKey().KeyChar.ToString(), out int res)) //if user enter number 0-9 parse it and return
            {
                return res;
            }

            return -1; //return -1 to indicate the wrong input
        }

        //used to update the output of the console when particular event occour
        public static void update()
        {
            if (updating) //if updating is true then dont update and return, it reduces shutering
                return;
            
            updating = true; //make the updating true to lock the update process
            Console.Clear();
            Console.WriteLine("|****Petrole Pump****|   Logged In: <Your Name>  Logout: 0\n");
            Console.WriteLine("<<Vehicles>>");
            foreach (var vehicle in vc.getVehicles()) //updating all vehicles
            {
                Console.WriteLine(vehicle.ToString());
            }
            Console.WriteLine("----------------------------------------------\n"+vc.ToString()+"\n\n<<Pumps>>");
            foreach(var pump in station.getPumps()) //updating all pumps
            {
                Console.WriteLine(pump.ToString());
            }

            Console.WriteLine("----------------------------------------------\n" + station.ToString());
            updating = false; //unlock the function
        }
    }
}

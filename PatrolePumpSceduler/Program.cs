using PatroleStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vehicles;

namespace PatrolePumpSceduler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(@"|============================================|");
            Console.WriteLine(@"|                                            |");
            Console.WriteLine(@"|          ***Petrole Pump***                |");
            Console.WriteLine(@"|                                            |");
            Console.WriteLine(@"|============================================|");

            Console.WriteLine("\n***Welcome To Petrole Pump***");
            Console.WriteLine("Please enter login details to continue!");
            Console.Write("Username: ");
            string username = Console.ReadLine(); //reading the username
            Console.Write("Password: ");
            string password = Console.ReadLine(); //reading the password

            if (username != "admin" && password != "1234") //if username and password did not matched
            {
                Console.WriteLine("Username or Password is incorrect!");
                return;
            }

            Console.WriteLine("Loading...");
            VehicleContainer vc = new VehicleContainer(); //calculate vehicle container
            Station station = new Station(3.5f); //calculate station
            IO.init(vc, station); //init input and output

            while (true)
            {
                int input = IO.getInput(); //get the input
                if (input == 0) { //if inout is 0 means it is logout
                    break; //get out of loop
                }
                if (input != -1 && (input > 0 && input < 10)) { //if the user input correct number
                    Vehicle v = vc.getVehicleToServed(); //get the vehicle to serve
                    station.serveVehicle(v, input);//serve the vehicle to pump
                }
            }

            Console.WriteLine("Thank you for working on Petrole Pump!");
            Console.WriteLine("Todays total earning: ",station.Commission, "£"); //get total comissioned
        }
    }
}

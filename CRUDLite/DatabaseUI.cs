using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;


namespace CRUDLiteLibrary
{
    public class DatabaseUI
    {
        //main methods
        public static void RunProgram()
        {
            do
            {
                DisplayOptionsToConsole();
                int option = AskUserForValidInput("\nWhat Would you like to do? (Select 0 to 5): ");
                DatabaseUI.RunSwitch(option);
                Console.ReadLine();
            } while (runAgain("Do you want to continue? (yes or no): "));

        }
        public static void RunSwitch(int option)
        {
            DataBaseLogic.CreateTable();

            switch (option)
            {
                case 0:
                    ExitProgram();
                    break;
                case 1:
                    ViewRecord();
                    break;
                case 2:
                    InsertRecord();
                    break;
                case 3:
                    UpdateRecord();
                    break;
                case 4:
                    DeleteRecord();
                    break;
                case 5:
                    DisplayTotalHours();
                    break;

            }
        }
        //Switch method
        private static void ExitProgram()
        {
            Console.WriteLine("Have a great day my friend!");
            Console.ReadLine();
            Environment.Exit(0);
        }
        private static void ViewRecord()
        {
            using (var conn = DataBaseLogic.CreateConnection())
            {
                using (var cmd = conn.CreateCommand())
                {


                    cmd.CommandText = "SELECT * FROM CodingTracker";
                    SQLiteDataReader rdr = cmd.ExecuteReader();
                    Console.WriteLine("\nThese are you existing records......");
                    Console.WriteLine("------------------------------------");
                    Console.WriteLine(string.Format("{0,-10} | {1,-10} | {2,5}", rdr.GetName(0), rdr.GetName(1), rdr.GetName(2)));
                    Console.WriteLine("------------------------------------");
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            Console.WriteLine(string.Format("{0,-10} | {1,-10} | {2,5}", rdr.GetValue(0), rdr.GetValue(1), rdr.GetValue(2)));
                        }
                        Console.WriteLine("------------------------------------");

                    }
                    else
                    {
                        Console.WriteLine("No records found.\n");
                    }
                    
                }

            }

        }
        private static void InsertRecord()
        {
            using (var conn = DataBaseLogic.CreateConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    ViewRecord();
                    (string date, double time) = AskForDataRecordInput();
                    cmd.CommandText =
                                    @"INSERT INTO CodingTracker 
                                        (Date, Hour)
                                        VALUES 
                                        (@Date, @Hour)";

                    cmd.Parameters.AddWithValue("@Date", date);
                    cmd.Parameters.AddWithValue("@Hour", time);

                    int row = cmd.ExecuteNonQuery();
                    if (row == 0)
                    {
                        Console.WriteLine("\nNo record is has been created");
                    }
                    else
                    {
                        Console.Write("\nA new record has been created...\n");
                        ViewRecord();
                    }

                }
            }
        }
        private static void UpdateRecord()
        {
            using (var conn = DataBaseLogic.CreateConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    ViewRecord();
                    Console.Write("\nEnter the ID number to update the associated records: ");
                    int Id = ConvertToInteger(Console.ReadLine());
                    cmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM CodingTracker WHERE Id = {Id})";
                    int checkQuery = Convert.ToInt32(cmd.ExecuteScalar());
                    if (checkQuery == 0)
                    {
                        Console.WriteLine($"\nNo Record exist yet.");
                        Console.ReadLine();
                    }
                    else
                    {
                        (string date, double hour) = AskForDataRecordInput();
                        cmd.CommandText = $"UPDATE CodingTracker SET Date = {date}, Hour = {hour} Where Id = {Id}";
                        int row = cmd.ExecuteNonQuery();
                        if (row == 0)
                        {
                            Console.WriteLine($"\nRecord with Id {Id} does not exist.. create a record first..");
                            Console.ReadLine();
                        }
                        else
                        {
                            Console.Write("\nSelected row has been updated...");
                            ViewRecord();
                        }
                    }


                }
            }
        }
        private static void DeleteRecord()
        {
            using (var conn = DataBaseLogic.CreateConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    ViewRecord();
                    Console.Write("\nEnter the ID number to update the associated records: ");
                    int Id = ConvertToInteger(Console.ReadLine());
                    cmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM CodingTracker WHERE Id = {Id})";
                    int checkQuery = Convert.ToInt32(cmd.ExecuteScalar());

                    if (checkQuery == 0)
                    {
                        Console.WriteLine($"\nRecord with Id {Id} does not exist..");
                        Console.ReadLine();
                    }
                    else
                    {
                        cmd.CommandText = $"DELETE FROM CodingTracker WHERE Id = {Id}";
                        int row = cmd.ExecuteNonQuery();

                        if (row == 0)
                        {
                            Console.WriteLine($"\nRecord with Id {Id} does not exist.. create a record first..");
                        }
                        else
                        {
                            Console.Write("\nSelected row has been deleted...");
                            ViewRecord();
                        }
                    }

                }
            }
        }
        private static void DisplayTotalHours()
        {
            using (var conn = DataBaseLogic.CreateConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    ViewRecord();
                    cmd.CommandText = "SELECT SUM(Hour) FROM CodingTracker";

                    var temp = cmd.ExecuteScalar();

                    double total = Convert.IsDBNull(temp) ? 0 : (double) temp;

                    if (total == 0)
                    {
                        Console.WriteLine("\nError, You probably do not have any hours yet..");
                    }
                    else
                    {
                        Console.Write($"\nYou have accumulated a total of {total} hours in coding. Please keep it up!");
                    }
                }
            }
        }
        //input method
        private static (string date, double time) AskForDataRecordInput()
        {

            RecordProperty record = new RecordProperty();

            Console.Write("\nUpdated current Date....");
            record.Date = DateTime.Now.ToString(format: "yyyy-MM-dd");

            Console.Write("\nInsert Time: ");
            string? input = Console.ReadLine();
            record.Time = ConvertToDouble(input);

            return (record.Date, record.Time);
        }
        private static int ConvertToInteger(string? input)
        {
            int output;
            while (int.TryParse(input, out output) == false)
            {
                Console.Write("\nInvalid input..Looking for an integer: ");
                input = Console.ReadLine();
            }
            return output;
        }
        private static int AskUserForValidInput(string message)
        {
            Console.Write(message);
            int inputInteger;
            do
            {
                string? input = Console.ReadLine();
                inputInteger = ConvertToInteger(input);

            } while (ValidateInput(inputInteger) == false);

            return inputInteger;
        }
        private static bool ValidateInput(int input)
        {
            if (input < 0 || input > 5)
            {
                Console.Write("\nPlease select the option within range(0 - 5): ");
                return false;
            }
            return true;
        }
        private static void DisplayOptionsToConsole()
        {
            Console.WriteLine("\nMAIN MENU\n");
            Console.WriteLine("Type 0 to Close Application\n" +
                              "Type 1 to View All Records.\n" +
                              "Type 2 to Insert Record.\n" +
                              "Type 3 to Update Record.\n" +
                              "Type 4 to Delete Record.\n" +
                              "Type 5 to Show Total Hours.");
        }
        private static bool runAgain(string message)
        {
            Console.Write(message);
            string? input = Console.ReadLine();
            if (input.ToLower() == "yes")
            {
                Console.Clear();
                return true;
            }
            return false;
        }
        private static double ConvertToDouble(string? input)
        {
            double output;
            while (double.TryParse(input, out output) == false)
            {
                Console.Write("\nInvalid input..Looking for an number: ");
                input = Console.ReadLine();
            }
            return output;
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            
            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName != null;
        }


        //// TODO Items: ////
        
        // TODO: Allow any of the CRUD operations to occur here
        // Done
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {


            switch (crudOperation)
            {
                case "create":
                    db.Employees.InsertOnSubmit(employee);
                    db.SubmitChanges();
                    break;

                case "read":
                    var employeeQuery = from e in db.Employees
                               where e.EmployeeId == employee.EmployeeId
                               select e;
                    break;

                case "update":
                    Employee employeeFromDb = null;

                    employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).Single();

                    employeeFromDb.FirstName = employee.FirstName;
                    employeeFromDb.LastName = employee.LastName;
                    employeeFromDb.UserName = employee.UserName;
                    employeeFromDb.Password = employee.Password;
                    employeeFromDb.EmployeeNumber = employee.EmployeeNumber;
                    employeeFromDb.Email = employee.Email;

                    db.SubmitChanges();
                    break;

                case "delete":
                    db.Employees.DeleteOnSubmit(employee);
                    db.SubmitChanges();
                    break;
            }

        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            try
            {
                db.Animals.InsertOnSubmit(animal);
                db.SubmitChanges();
            }
            catch(Exception)
            {
                Console.Clear();
                UserInterface.DisplayUserOptions("Animal addition unsuccessful please try again");
                return;
            }

        }

        internal static Animal GetAnimalByID(int id)
        {
            Animal animalById = new Animal();
            try
            {
                animalById = (Animal)(from a in db.Animals
                              where a.AnimalId == id
                              select a);
                return animalById;
            }
            catch(Exception)
            {
                Console.Clear();
                UserInterface.DisplayUserOptions("There is no animal with that animal ID. Please try again.");
                return animalById;
            }
            
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            foreach (KeyValuePair<int, string> kvp in updates)
            {
                Animal animalDb = db.Animals.Where(a => a.AnimalId == animalId).Single(); ;
                switch (kvp.Key)
                {
                    case 1:
                        Category category = db.Categories.Where(a => a.Name == kvp.Value).Single();
                        animalDb.CategoryId = category.CategoryId;
                        db.SubmitChanges();
                        break;
                    case 2:
                        animalDb.Name = kvp.Value;
                        db.SubmitChanges();
                        break;
                    case 3:
                        animalDb.Age = Int32.Parse(kvp.Value);
                        db.SubmitChanges();
                        break;
                    case 4:
                        animalDb.Demeanor = kvp.Value;
                        db.SubmitChanges();
                        break;
                    case 5:
                        animalDb.KidFriendly = Convert.ToBoolean(kvp.Value);
                        db.SubmitChanges();
                        break;
                    case 6:
                        animalDb.PetFriendly = Convert.ToBoolean(kvp.Value);
                        db.SubmitChanges();
                        break;
                    case 7:
                        animalDb.Weight = Int32.Parse(kvp.Value);
                        db.SubmitChanges();
                        break;
                    case 8:
                        animalDb.AnimalId = Int32.Parse(kvp.Value);
                        db.SubmitChanges();
                        break;
                    default:
                        UserInterface.DisplayUserOptions("Input not recognized please try agian");
                        break;
                }
            }
        }

        internal static void RemoveAnimal(Animal animal)
        {
            try
            {
                db.Animals.DeleteOnSubmit(animal);
                db.SubmitChanges();
            }
            catch
            {
                Console.Clear();
                UserInterface.DisplayUserOptions("Couldn't remove the animal. Please try again.");
                return;
            }
        }
        
        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            IQueryable<Animal> animals = db.Animals;

            foreach (KeyValuePair<int, string> kpv in updates)
            {

                switch (kpv.Key)
                {

                    case 1:
                        animals = animals.Where(a => a.AnimalId == Int32.Parse(kpv.Value));

                        break;
                    case 2:
                        animals = animals.Where(a => a.Name == kpv.Value);

                        break;
                    case 3:
                        animals = animals.Where(a => a.Weight == Int32.Parse(kpv.Value));

                        break;
                    case 4:
                        animals = animals.Where(a => a.Age == Int32.Parse(kpv.Value));

                        break;
                    case 5:
                        animals = animals.Where(a => a.Demeanor == kpv.Value);

                        break;
                    case 6:
                        animals = animals.Where(a => a.KidFriendly == bool.Parse(kpv.Value));

                        break;
                    case 7:
                        animals = animals.Where(a => a.PetFriendly == bool.Parse(kpv.Value));

                        break;
                    case 8:
                        animals = animals.Where(a => a.CategoryId == GetCategoryId(kpv.Value));

                        break;
                }

            }

            return animals;
        }
         
        // TODO: Misc Animal Things
        // Done
        internal static int GetCategoryId(string categoryName)
        {
            return db.Categories.Where(c => c.Name == categoryName).Select(c => c.CategoryId).FirstOrDefault();
        }
        
        internal static Room GetRoom(int animalId)
        {
            return db.Rooms.Where(r => r.AnimalId == animalId).FirstOrDefault();
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            return db.DietPlans.Where(d => d.Name == dietPlanName).Select(d => d.DietPlanId).FirstOrDefault();
        }

        // TODO: Adoption CRUD Operations
        // Done
        internal static void Adopt(Animal animal, Client client)
        {
            Adoption adoption = new Adoption();
            adoption.AnimalId = animal.AnimalId;
            adoption.ClientId = client.ClientId;
            adoption.ApprovalStatus = "Adoption is Pending";

            db.Adoptions.InsertOnSubmit(adoption);
            db.SubmitChanges();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            return db.Adoptions.Where(a => a.ApprovalStatus == "Adoption is Pending");
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            adoption.ApprovalStatus = isAdopted ? "Approved" : "Declined";
            db.SubmitChanges();
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            Adoption adoption = db.Adoptions.Where(a => a.AnimalId == animalId && a.ClientId == clientId).FirstOrDefault();

            db.Adoptions.DeleteOnSubmit(adoption);
            db.SubmitChanges();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            var vaccineShot = db.AnimalShots.Where(s => s.AnimalId == animal.AnimalId);

            return vaccineShot;
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            AnimalShot animalShot = new AnimalShot();

            var vaccineShot = db.Shots.Where(s => s.Name == shotName).Select(s => s.ShotId).SingleOrDefault();

            animalShot.AnimalId = animal.AnimalId;
            animalShot.ShotId = vaccineShot;
            animalShot.DateReceived = DateTime.Now;

            db.AnimalShots.InsertOnSubmit(animalShot);
            db.SubmitChanges();
        }
    }
}
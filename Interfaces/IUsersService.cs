using Tasks.Models;
using System.Collections.Generic;

namespace Tasks.Interfaces;

public interface IUserServices
{
    List<User> GetAll();

    User GetById(int id);
    
    int Add(User newUser);
 
    bool Update(int id, User newUser);
    
    bool Delete(int id);
}
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieShop.Services
{
    public interface ICryptoService
    {
        string CreateSalt(); //generate salt
        string HashPassword(string password, string salt);
    }
}

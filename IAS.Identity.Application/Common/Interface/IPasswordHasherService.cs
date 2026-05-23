using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAS.Identity.Application.Common.Interface;

public interface IPasswordHasherService
{
    /// <summary>Hashes a plain-text password. Returns a self-contained hash string.</summary>
    string Hash(string password);

    /// <summary>Verifies a plain-text password against a stored hash.</summary>
    bool Verify(string password, string hashedPassword);
}
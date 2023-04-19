using System;
namespace Chick_fil_A.Services
{
    public interface IIntent
    {
        bool IsTagDiscovered();
        string Type();
    }
}


namespace AEBackendProject.Common.Exceptions
{
    public class ShipAlreadyAssignedException : Exception
    {
        public ShipAlreadyAssignedException(string message) : base(message)
        {
        }
    }
}

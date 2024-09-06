namespace AEBackendProject.Repositories
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IShipRepository ShipRepository { get; }
        IPortRepository PortRepository { get; }
        IUserShipRepository UserShipRepository { get; }
        Task<int> CompleteAsync();
    }
}

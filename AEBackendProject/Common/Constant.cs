namespace AEBackendProject.Common
{
    public static class Constant
    {
        public const double KnotInKM = 1.852;
        public const double KnotInMiles = 1.15078;
        public const double EarthRadiusKm = 6371.0;
        public const double EarthRadiusMiles = 3963.0;

        public const int Ok = 200;
        public const int BadRequest = 400;
        public const int NotFound = 404;
        public const int ServerError = 500;

        public const string GlobalExceptionMessage = "An unexpected error occurred. please check the log or contact administrator";
    }
}

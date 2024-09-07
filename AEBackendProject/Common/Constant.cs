namespace AEBackendProject.Common
{
    public static class Constant
    {
        public const double Knot = 1.852;
        public const double EarthRadiusKm = 6371.0;

        public const int Ok = 200;
        public const int BadRequest = 400;
        public const int NotFound = 404;
        public const int ServerError = 500;

        public const string GlobalExceptionMessage = "An unexpected error occurred. please check the log or contact administrator";
    }
}

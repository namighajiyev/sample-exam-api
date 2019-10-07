namespace SampleExam.Common
{
    public static class Constants
    {
        public const int API_TOKEN_MINUTES = 60;
        public const int PASSWORD_MIN_LENGTH = 8;
        public static string NOT_FOUND = "not found";
        public static string IN_USE = "in use";

        public static string API_JWT_KEY = "ASPNETCORE_SampleExamApi_Jwt_Key";

        public static string CONN_STRING_KEY = "ASPNETCORE_SampleExamApi_ConnectionString";

        public static string VALIDATION_ERRORS_KEY = "validation errors";

        public static int USER_FIRSTNAME_LEN = 100;
        public static int USER_LASTNAME_LEN = 100;

        public static int USER_MIDDLENAME_LEN = 100;

        public static int USER_EMAIL_LEN = 100;
        public static int REFRESH_TOKEN_LEN = 128;

        public static int EXAM_TITLE_LEN = 150;
        public static int EXAM_DESCRIPTIION_LEN = 1000;

        public static int EXAM_TIME_IN_MINUTES_MAX = 240;
        public static int EXAM_TIME_IN_MINUTES_MIN = 1;

        public static int EXAM_PASS_PERCENTAGE_MAX = 100;
        public static int EXAM_PASS_PERCENTAGE_MIN = 1;

        public static int ANSWEROPTION_TEXT_LEN = 500;

        public static int TAG_TEXT_LEN = 50;

        public static int QUESTION_TEXT_LEN = 3000;
        public static int FETCH_OFFSET = 0;
        internal static int FETCH_LIMIT = 20;

        public const string ASCII_PRINTABLE_CHARS_REGEX = "^[\x20-\x7E]+$";

        public const string NOT_BEGINING_OR_ENDING_WITH_ASCII_SPACE_REGEX = "^\x20+.*$|^.*\x20+$";
    }
}
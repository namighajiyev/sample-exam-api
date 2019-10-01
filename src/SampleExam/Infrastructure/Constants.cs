namespace SampleExam.Infrastructure
{
    public static class Constants
    {
        public static string NOT_FOUND = "not found";
        public static string IN_USE = "in use";

        public static string VALIDATION_ERRORS_KEY = "validation errors";

        public static int USER_FIRSTNAME_LEN = 100;
        public static int USER_LASTNAME_LEN = 100;

        public static int USER_MIDDLENAME_LEN = 100;

        public static int USER_EMAIL_LEN = 100;
        public static int REFRESH_TOKEN_LEN = 128;

        public static int EXAM_TITLE_LEN = 150;
        public static int EXAM_DESCRIPTIION_LEN = 1000;
        public static int ANSWEROPTION_TEXT_LEN = 500;

        public static int TAG_TEXT_LEN = 50;

        public static int QUESTION_TEXT_LEN = 3000;
        public static int FETCH_OFFSET = 0;
        internal static int FETCH_LIMIT = 20;
    }
}
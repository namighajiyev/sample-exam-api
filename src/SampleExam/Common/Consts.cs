namespace SampleExam.Common
{
    public static class Consts
    {
        public const int PASSWORD_MIN_LENGTH = 8;

        public const string ASCII_PRINTABLE_CHARS_REGEX = "^[\x20-\x7E]+$";

        public const string NOT_BEGINING_OR_ENDING_WITH_ASCII_SPACE_REGEX = "^\x20+.*$|^.*\x20+$";
    }
}
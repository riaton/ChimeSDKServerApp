namespace ChimeApp.Domain;

public static class CommonResult {
    public const int OK = 200;
    public const int ValidateError = 400;
    public const int InternalServerError = 500;
    private static IDictionary<string, string> _header = new Dictionary<string, string>(){
        {"Access-Control-Allow-Headers", "Content-Type"},
        {"Access-Control-Allow-Origin", "*"},
        {"Access-Control-Allow-Methods", "OPTIONS,POST,GET"}
    };
    public static IDictionary<string, string> ResponseHeader => _header;

    public static string CreateStatusMessage(int resultCode){
        switch(resultCode){
            case OK:
                return "OK";
            case ValidateError:
                return "Validation Error";
            case InternalServerError:
                return "Internal Server Error";
            default:
                return "What is this error?";
        }
    }
}
using IBKR_Service.Handlers;

namespace IBKR_Service
{
    public class ErrorResponse 
    {
        public string error { get; set; }
    }

    public class ConfirmationResponse 
    {
        public IEnumerable<string> message { get; set; }
        public string id { get; set; }

//        [
//    {
//        "id": "c25fbf4d-9649-4ce1-aa57-7d90ce40bb12",
//        "message": [
//            "You are submitting an order without market data. We strongly recommend against this as it may result in erroneous and unexpected trades.\nAre you sure you want to submit this order?"
//        ],
//        "isSuppressed": false,
//        "messageIds": [
//            "o354"
//        ],
//        "messageOptions": [
//            "Yes",
//            "No"
//        ]
//    }
//]
    }

    public class SuccessfulResponse 
    {
        public string order_id { get; set; }
        public string order_status { get; set; }

        //order_id": "1104414515",
        //"order_status": "PreSubmitted",
        //"encrypt_message": "1"
    }

    public class PnlResponse
    {
        public string order_id { get; set; }
        public string order_status { get; set; }

        //order_id": "1104414515",
        //"order_status": "PreSubmitted",
        //"encrypt_message": "1"
    }

    public class ReplyRequest() {
        public bool confirmed { get; set; }
    }
}


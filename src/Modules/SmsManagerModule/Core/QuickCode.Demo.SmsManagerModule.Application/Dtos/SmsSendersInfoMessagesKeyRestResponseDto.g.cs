//------------------------------------------------------------------------------  
// <auto-generated>                                                               
// This code was generated by QuickCode.                                          
// Runtime Version:1.0                                                            
//                                                                                
// Changes to this file may cause incorrect behavior and will be lost if          
// the code is regenerated.                                                       
// </auto-generated>                                                              
//------------------------------------------------------------------------------  
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace QuickCode.Demo.SmsManagerModule.Application.Dtos
{
    public record SmsSendersInfoMessagesKeyRestResponseDto
    {
        public int Id { get; init; }
        public int SmsSenderId { get; init; }
        public int InfoTypeId { get; init; }
        public string GsmNumber { get; init; }
        public string Message { get; init; }
        public DateTime? MessageDate { get; init; }
        public string MessageSid { get; init; }
        public int DailyCounter { get; init; }
    }
}
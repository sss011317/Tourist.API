using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Stateless;
namespace Tourist.API.Models
{
    public enum OrderStateEnum 
        //狀態是指可以穩定維持事物某種具體過程的表現形式，一般來說會用名詞或形容詞描述
    {
        Pending, //訂單已生成
        Processing, //支付處理中
        Completed, //交易成功
        Declined, //交易失敗
        Cancelled, //訂單取消
        Refund //已退款
    }
    public enum OrderStateTriggerEnum
        //而動作更多的是在描述操作，一般來說這種操作是不可持續的，所以用及動詞表示
    {
        PlaceOrder, //支付
        Approve, //支付成功
        Reject, //支付失敗
        Cancel, //取消
        Return //退貨
    }
    public class Order
    {
        public Order()
        {
            StateMachineInit(); //每次創建訂單得時候都會初始化狀態機 
        }

        [Key]
        public Guid Id { get; set; }
        public String UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<LineItem> OrderItems { get; set; }
        public OrderStateEnum State { get; set; }
        public DateTime CreateDateUTC { get; set; }
        public string TransactionMetadata { get; set; }


        StateMachine<OrderStateEnum, OrderStateTriggerEnum> _machine;
        /// <summary>
        /// -------------------------------------------------------------------------------------------------------------------------|
        /// 現態                 |     條件                       |      動作                              |  次態                   |
        /// -------------------------------------------------------------------------------------------------------------------------|
        /// 訂單生成   Pending   |    購物車非空，用戶點擊結算    |     點擊支付  PlaceOrder               |  支付處理中             |
        /// 訂單取消   Cancelled |    訂單合法，買家取消          |       -                                |   -                     |
        /// 支付處理   Processing|    訂單合法，第三方處理支付    |     支付失敗 Rejcet、支付成功 Approve  |  交易已失敗、交易已成功 |
        /// 交易已失敗 Declined  |    第三方收付款失敗            |     重新支付                           |  支付處理中             |
        /// 交易已成功 Completed |    第三方收付款成功            |     買家退貨  Return                   |  已退單                 |
        /// 已退單     Refound   |    訂單核發驗貨成功，買家退貨  |      -                                 |   -                     |
        /// -------------------------------------------------------------------------------------------------------------------------|
        /// </summary>


        public void PaymentProcessing()
        {
            _machine.Fire(OrderStateTriggerEnum.PlaceOrder);
        }
        public void PaymentApprove()
        {
            _machine.Fire(OrderStateTriggerEnum.Approve);
        }
        public void PaymentReject()
        {
            _machine.Fire(OrderStateTriggerEnum.Reject);
        }
        private void StateMachineInit() //狀態機
        {
            //1.給私有成員變數初始化狀態機
            //_machine = new StateMachine<OrderStateEnum, OrderStateTriggerEnum>  
            //    (OrderStateEnum.Pending);
            _machine = new StateMachine<OrderStateEnum, OrderStateTriggerEnum>
                //把原本寫死的兩個狀態改為命名函數
                (() =>State, s=> State = s);
            //2.配置狀態機觸發動作與狀態轉換
            _machine.Configure(OrderStateEnum.Pending)//訂單生成後
                //如果觸發狀態為PlaceOreder點擊支付，將會觸發第二個狀態Processing支付處理
                 .Permit(OrderStateTriggerEnum.PlaceOrder, OrderStateEnum.Processing)
                 //如果觸發狀態為Cancel取消，將會觸發第二個狀態Canceled訂單取消
                 .Permit(OrderStateTriggerEnum.Cancel, OrderStateEnum.Cancelled);

            _machine.Configure(OrderStateEnum.Processing)//訂單支付處理後
                 //如果觸發狀態為Approve第三方收款成功，將觸發第二個狀態Completed交易已成功
                .Permit(OrderStateTriggerEnum.Approve, OrderStateEnum.Completed)
                //如果觸發狀態為Approve第三方收款失敗，將觸發第二個狀態Completed交易已失敗
                .Permit(OrderStateTriggerEnum.Reject, OrderStateEnum.Declined);

            _machine.Configure(OrderStateEnum.Declined) //交易失敗後
             //如果觸發狀態為PlaceOrder重新提交訂單，將觸發第二個狀態Processing訂單支付處理
                .Permit(OrderStateTriggerEnum.PlaceOrder, OrderStateEnum.Processing);
            
            _machine.Configure(OrderStateEnum.Completed)//交易成功後
                //如果觸發狀態為Rrturn退款，將觸發第二個狀態Refund退貨
                .Permit(OrderStateTriggerEnum.Return, OrderStateEnum.Refund);
        }
    }
}

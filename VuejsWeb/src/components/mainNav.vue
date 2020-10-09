<template>
    <div class="mainNav container">
        <b-navbar toggleable="lg" type="dark" variant="danger">

            <b-navbar-toggle target="nav-collapse"></b-navbar-toggle>

            <b-collapse id="nav-collapse" is-nav>
                <b-nav-item-dropdown text="據點管理">
                    <b-dropdown-item to="/navStronghold_position">據點資訊設定</b-dropdown-item>
                    <b-dropdown-item to="/navStronghold_alter">調度停車場資訊設定</b-dropdown-item>
                    <b-dropdown-item to="/navStronghold_special">停車便利付停車場設定</b-dropdown-item>
                </b-nav-item-dropdown>
                <b-nav-item-dropdown text="車輛管理">
                    <b-dropdown-item to="/navCar_center">車輛中控台</b-dropdown-item>
                    <b-dropdown-item to="/">456</b-dropdown-item>
                </b-nav-item-dropdown>
                <b-nav-item-dropdown text="卡片管理">
                    <b-dropdown-item to="/navCard_admin">萬用卡管理</b-dropdown-item>
                    <b-dropdown-item to="/navCard_setting">卡號發送設定</b-dropdown-item>
                </b-nav-item-dropdown>
                <b-nav-item-dropdown text="系統管理">
                    <b-dropdown-item to="/navSystem_message">訊息記錄查詢</b-dropdown-item>
                    <b-dropdown-item to="/navSystem_time">車輛管理時間查詢</b-dropdown-item>
                </b-nav-item-dropdown>
                <b-nav-item-dropdown text="地圖監控">
                    <b-dropdown-item to="#">EN</b-dropdown-item>
                    <b-dropdown-item to="#">ES</b-dropdown-item>
                    <b-dropdown-item to="#">RU</b-dropdown-item>
                    <b-dropdown-item to="#">FA</b-dropdown-item>
                </b-nav-item-dropdown>
                <b-nav-item-dropdown text="合約管理">
                    <b-dropdown-item to="/navContract_inquire">預約(合約)資料查詢</b-dropdown-item>
                    <b-dropdown-item to="/navContract_history">訂單紀錄歷程查詢</b-dropdown-item>
                    <b-dropdown-item to="/navContract_motorcycle">機車合約修改</b-dropdown-item>
                    <b-dropdown-item to="/navContract_car">汽車合約修改</b-dropdown-item>
                    <b-dropdown-item to="/navContract_discount">時數折抵</b-dropdown-item>
                    <b-dropdown-item to="/navContract_force">強制延長用車(缺)</b-dropdown-item>
                    <b-dropdown-item to="/navContract_add">人工新增預約</b-dropdown-item>
                    <b-dropdown-item to="/navContract_car">強制取還車(作廢)(缺)</b-dropdown-item>
                </b-nav-item-dropdown>
                <b-nav-item-dropdown text="後台管理">
                    <b-dropdown-item to="/navBackstage_force">強制刷卡取還</b-dropdown-item>
                    <b-dropdown-item to="/navBackstage_holiday">平假日維護</b-dropdown-item>
                    <b-dropdown-item to="/navBackstage_release">卡號解除</b-dropdown-item>
                    <b-dropdown-item to="/navBackstage_creditCard">信用卡解綁</b-dropdown-item>
                    <b-dropdown-item to="/navBackstage_short">短租補傳</b-dropdown-item>
                </b-nav-item-dropdown>
                <b-nav-item-dropdown text="報表專區">
                    <b-dropdown-item to="/navReport_worker">整備人員報表查詢</b-dropdown-item>
                    <b-dropdown-item to="/navReport_car">車況回饋查詢</b-dropdown-item>
                    <b-dropdown-item to="/navReport_ecpay">綠界交易紀錄查詢</b-dropdown-item>
                    <b-dropdown-item to="/navReport_summary">月租總表查詢</b-dropdown-item>
                    <b-dropdown-item to="/navReport_monthly">月租報表</b-dropdown-item>
                    <!-- <b-dropdown-item to="/navReport_parking">進出停車場明細</b-dropdown-item> -->
                    <b-dropdown-item to="/navReport_fee">代收停車費明細</b-dropdown-item>
                </b-nav-item-dropdown>
                <b-nav-item-dropdown text="推播管理">
                    <b-dropdown-item to="/navDiscount_setting">APP優惠訊息推播</b-dropdown-item>
                </b-nav-item-dropdown>
                <b-nav-item-dropdown text="會員管理">
                    <b-dropdown-item to="/navMember_member">會員審核</b-dropdown-item>
                </b-nav-item-dropdown>
                <b-nav-item-dropdown text="帳號管理" right>
                    <b-dropdown-item to="/navAccount_franchise">加盟業者維護</b-dropdown-item>
                    <b-dropdown-item to="/navAccount_group">功能群組維護</b-dropdown-item>
                    <b-dropdown-item to="/navAccount_program">功能程式維護</b-dropdown-item>
                    <b-dropdown-item to="/navAccount_userGroup">使用者群組維護</b-dropdown-item>
                    <b-dropdown-item to="/navAccount_user">使用者維護</b-dropdown-item>
                </b-nav-item-dropdown>
                <!-- <b-nav-item-dropdown text="客服專區" right>
                    <b-dropdown-item to="/navContract_contact">合約/中控台</b-dropdown-item>
                </b-nav-item-dropdown>
                <b-nav-item-dropdown text="儀表板" right>
                    <b-dropdown-item to="#">EN</b-dropdown-item>
                    <b-dropdown-item to="#">ES</b-dropdown-item>
                    <b-dropdown-item to="#">RU</b-dropdown-item>
                    <b-dropdown-item to="#">FA</b-dropdown-item>
                </b-nav-item-dropdown> -->

            </b-collapse>
        </b-navbar>
    </div>
</template>

<script>
export default {
    name: 'mainNav',
    props: {
        msg: String
    }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">
ul {
    list-style-type: none;
    padding: 0;
}
li {
    list-style: none;
    // display: inline-block;
    margin: 0;
}
.mainNav nav{
    border-radius: 5px;
    padding: .5rem;
}

</style>

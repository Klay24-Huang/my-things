import Vue from 'vue'
import VueRouter from 'vue-router'
import login from '../views/login.vue'

Vue.use(VueRouter)

const routes = [
    {
　　　　path: '*',
　　　　redirect: '/'
　　},
    {
        path: '/',
        name: 'login',
        component: login
    },
    //===修改密碼===
    {
        path: '/edit_password',
        name: 'edit_password',
        component: () => import(/* webpackChunkName: "about" */ '../views/edit_password.vue')
    },
    //===帳號管理===
    {
        // 加盟業者維護
        path: '/navAccount_franchise',
        name: 'navAccount_franchise',
        // route level code-splitting
        // this generates a separate chunk (about.[hash].js) for this route
        // which is lazy-loaded when the route is visited.
        component: () => import(/* webpackChunkName: "about" */ '../views/navAccount_franchise.vue')
    },{
        // 功能群組維護
        path: '/navAccount_group',
        name: 'navAccount_group',
        component: () => import(/* webpackChunkName: "about" */ '../views/navAccount_group.vue')
    },{
        // 功能程式維護
        path: '/navAccount_program',
        name: 'navAccount_program',
        component: () => import(/* webpackChunkName: "about" */ '../views/navAccount_program.vue')
    },{
        // 使用者群組維護
        path: '/navAccount_userGroup',
        name: 'navAccount_userGroup',
        component: () => import(/* webpackChunkName: "about" */ '../views/navAccount_userGroup.vue')
    },{
        // 使用者維護
        path: '/navAccount_user',
        name: 'navAccount_user',
        component: () => import(/* webpackChunkName: "about" */ '../views/navAccount_user.vue')
    },
    //===會員管理===
    {
        // 會員審核
        path: '/navMember_member',
        name: 'navMember_member',
        component: () => import(/* webpackChunkName: "about" */ '../views/navMember_member.vue')
    },
    //===客服專區===
    {
        // 合約中控台
        path: '/navService_contact',
        name: 'navService_contact',
        component: () => import(/* webpackChunkName: "about" */ '../views/navService_contact.vue')
    },
    //===優惠管理===
    {
        // APP優惠訊息推播
        path: '/navDiscount_setting',
        name: 'navDiscount_setting',
        component: () => import(/* webpackChunkName: "about" */ '../views/navDiscount_setting.vue')
    },
    // ===報表===
    {
        // 整備人員報表查詢
        path: '/navReport_worker',
        name: 'navReport_worker',
        component: () => import(/* webpackChunkName: "about" */ '../views/navReport_worker.vue')
    },
    {
        // 車況回饋查詢
        path: '/navReport_car',
        name: 'navReport_car',
        component: () => import(/* webpackChunkName: "about" */ '../views/navReport_car.vue')
    },
    {
        // 綠界交易紀錄查詢
        path: '/navReport_ecpay',
        name: 'navReport_ecpay',
        component: () => import(/* webpackChunkName: "about" */ '../views/navReport_ecpay.vue')
    },
    {
        // 月租總表查詢
        path: '/navReport_summary',
        name: 'navReport_summary',
        component: () => import(/* webpackChunkName: "about" */ '../views/navReport_summary.vue')
    },
    {
        // 月租報表
        path: '/navReport_monthly',
        name: 'navReport_monthly',
        component: () => import(/* webpackChunkName: "about" */ '../views/navReport_monthly.vue')
    },
    {
        // 代收停車費明細
        path: '/navReport_fee',
        name: 'navReport_fee',
        component: () => import(/* webpackChunkName: "about" */ '../views/navReport_fee.vue')
    },
    //===後台管理===
    {
        //強制刷卡取還
        path: '/navBackstage_force',
        name: 'navBackstage_force',
        component: () => import(/* webpackChunkName: "about" */ '../views/navBackstage_force.vue')
    },
    {
        //平假日維護
        path: '/navBackstage_holiday',
        name: 'navBackstage_holiday',
        component: () => import(/* webpackChunkName: "about" */ '../views/navBackstage_holiday.vue')
    },
    {
        //卡號解除
        path: '/navBackstage_release',
        name: 'navBackstage_release',
        component: () => import(/* webpackChunkName: "about" */ '../views/navBackstage_release.vue')
    },
    {
        //信用卡解綁
        path: '/navBackstage_creditCard',
        name: 'navBackstage_creditCard',
        component: () => import(/* webpackChunkName: "about" */ '../views/navBackstage_creditCard.vue')
    },
    {
        //短租補傳
        path: '/navBackstage_short',
        name: 'navBackstage_short',
        component: () => import(/* webpackChunkName: "about" */ '../views/navBackstage_short.vue')
    },
    // ===卡片管理===
    {
        //萬用卡管理
        path: '/navCard_admin',
        name: 'navCard_admin',
        component: () => import(/* webpackChunkName: "about" */ '../views/navCard_admin.vue')
    },
    {
        //卡號發送設定
        path: '/navCard_setting',
        name: 'navCard_setting',
        component: () => import(/* webpackChunkName: "about" */ '../views/navCard_setting.vue')
    },
    //===據點管理===
    {
        //據點資訊設定
        path: '/navStronghold_position',
        name: 'navStronghold_position',
        component: () => import(/* webpackChunkName: "about" */ '../views/navStronghold_position.vue')
    },
    {
        //調度停車場
        path: '/navStronghold_alter',
        name: 'navStronghold_alter',
        component: () => import(/* webpackChunkName: "about" */ '../views/navStronghold_alter.vue')
    },
    {
        //特約停車場
        path: '/navStronghold_special',
        name: 'navStronghold_special',
        component: () => import(/* webpackChunkName: "about" */ '../views/navStronghold_special.vue')
    },
    //===系統管理===
    {
        //訊息記錄查詢
        path: '/navSystem_message',
        name: 'navSystem_message',
        component: () => import(/* webpackChunkName: "about" */ '../views/navSystem_message.vue')
    },
    {
        //車輛管理時間查詢
        path: '/navSystem_time',
        name: 'navSystem_time',
        component: () => import(/* webpackChunkName: "about" */ '../views/navSystem_time.vue')
    },
    //===合約管理===
    {
        //預約(合約)資料查詢
        path: '/navContract_inquire',
        name: 'navContract_inquire',
        component: () => import(/* webpackChunkName: "about" */ '../views/navContract_inquire.vue')
    },
    {
        //訂單紀錄歷程查詢
        path: '/navContract_history',
        name: 'navContract_history',
        component: () => import(/* webpackChunkName: "about" */ '../views/navContract_history.vue')
    },
    {
        //機車合約修改
        path: '/navContract_motorcycle',
        name: 'navContract_motorcycle',
        component: () => import(/* webpackChunkName: "about" */ '../views/navContract_motorcycle.vue')
    },
    {
        //汽車合約修改
        path: '/navContract_car',
        name: 'navContract_car',
        component: () => import(/* webpackChunkName: "about" */ '../views/navContract_car.vue')
    },
    {
        //時數折抵
        path: '/navContract_discount',
        name: 'navContract_discount',
        component: () => import(/* webpackChunkName: "about" */ '../views/navContract_discount.vue')
    },
    {
        //強制延長用車
        path: '/navContract_force',
        name: 'navContract_force',
        component: () => import(/* webpackChunkName: "about" */ '../views/navContract_discount.vue')
    },
    {
        //人工新增預約
        path: '/navContract_add',
        name: 'navContract_add',
        component: () => import(/* webpackChunkName: "about" */ '../views/navContract_add.vue')
    },
    //===車輛管理===
    {
        //車輛中控台
        path: '/navCar_center',
        name: 'navCar_center',
        component: () => import(/* webpackChunkName: "about" */ '../views/navCar_center.vue')
    },

    {
        path: '/about',
        name: 'About',
        component: () => import(/* webpackChunkName: "about" */ '../views/About.vue')
    }

]

const router = new VueRouter({
    routes
})

export default router

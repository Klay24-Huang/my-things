<template>
    <div>
        <mainHeader/>
        <!-- breadcum -->
        <div class="container text-left">
            <p class="text-secondary my-2">
                合約管理 / 
                <font-awesome-icon icon="file-alt"></font-awesome-icon> 機車合約修改
            </p>
        </div>
        <main class="bg-gray-400">
            <mainNav/>
            <div class="container text-left">
                <b-card
                    header="機車合約修改"
                    header-text-variant="white"
                    header-bg-variant="secondary"
                    class="mt-4"
                    no-body
                >
                    <b-list-group flush>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>合約編號</label>
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-input type="text" trim></b-form-input>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="6" offset-sm="2">
                                        <b-button class="mr-2" variant="primary" @click.prevent="inquire()">查詢</b-button>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                    </b-list-group>        
                </b-card>
            </div>
            <!-- output -->
            <div class="container mt-4 text-left">
                <b-card
                    header="訂單基本資料"
                    header-text-variant="white"
                    header-bg-variant="secondary"
                    class="mt-4"
                    no-body
                >
                    
                    <b-list-group flush>
                        <b-list-group-item class="bg-gray-500">
                            <b-container fluid class="p-0">
                                    <b-row class="my-1 text-white" no-gutters>
                                        <b-col sm="2">
                                            訂單/合約明細
                                        </b-col>
                                    </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>合約編號</label>
                                    </b-col>
                                    <b-col sm="4">
                                        123456789
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>會員帳號</label>
                                    </b-col>
                                    <b-col sm="4">
                                        123456789
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>會員姓名</label>
                                    </b-col>
                                    <b-col sm="4">
                                        王大大
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>訂單專案</label>
                                    </b-col>
                                    <b-col sm="4">
                                        A00
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>前筆實際還車時間</label>
                                    </b-col>
                                    <b-col sm="4">
                                        123456789
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>實際取車時間</label>
                                    </b-col>
                                    <b-col sm="4">
                                        2020年09月09號09時09分
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-datepicker id="example-datepicker" placeholder="請選擇日期"></b-form-datepicker>
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-timepicker v-model="start" locale="en" :hour12="isHour12" class="ml-2"></b-form-timepicker>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>實際還車時間</label>
                                    </b-col>
                                    <b-col sm="4">
                                        2020年09月09號09時09分
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-datepicker id="example-datepicker2" placeholder="請選擇日期"></b-form-datepicker>
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-timepicker v-model="start" locale="en" :hour12="isHour12" class="ml-2"></b-form-timepicker>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>租金</label>
                                    </b-col>
                                    <b-col sm="4">
                                        123456789
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-input type="text"></b-form-input>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>一日租金</label>
                                    </b-col>
                                    <b-col sm="4">
                                        123456789
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>時數折抵</label>
                                    </b-col>
                                    <b-col sm="4">
                                        123456789
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-input type="text"></b-form-input>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>前筆還車里程</label>
                                    </b-col>
                                    <b-col sm="4">
                                        123456789
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>租車里程</label>
                                    </b-col>
                                    <b-col sm="4">
                                        123456789
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-input type="text"></b-form-input>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>還車里程</label>
                                    </b-col>
                                    <b-col sm="4">
                                        123456789
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-input type="text"></b-form-input>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>里程費用</label>
                                    </b-col>
                                    <b-col sm="4">
                                        123456789
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-input type="text" disabled="true"></b-form-input>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>逾時費用</label>
                                    </b-col>
                                    <b-col sm="4">
                                        00
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-input type="text"></b-form-input>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>ETag費用</label>
                                    </b-col>
                                    <b-col sm="4">
                                        00
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-input type="text" disabled="true"></b-form-input>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>結算金額</label>
                                    </b-col>
                                    <b-col sm="4">
                                        00
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-input type="text" disabled="true"></b-form-input>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>已付金額</label>
                                    </b-col>
                                    <b-col sm="4">
                                        00
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="6">
                                        <label>差額(原始結算金額-更改結算金額) 負數為刷卡，正數為刷退</label>
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-input type="text" disabled="true"></b-form-input>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="6">
                                        <label>修改原因</label>
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-select v-model="selected" :options="options"></b-form-select>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="6">
                                        <label>備註</label>
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-input type="text"></b-form-input>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item class="bg-gray-500">
                            <b-container fluid class="p-0">
                                    <b-row class="my-1 text-white" no-gutters>
                                        <b-col sm="2">
                                            新增待繳費用
                                        </b-col>
                                    </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>車輛備註</label>
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-input type="text"></b-form-input>
                                    </b-col>
                                    <b-col sm="1">
                                        元
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-input type="text" placeholder="備註"></b-form-input>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>清潔費</label>
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-input type="text"></b-form-input>
                                    </b-col>
                                    <b-col sm="1">
                                        元
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-input type="text" placeholder="備註"></b-form-input>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>物品損壞/遺失</label>
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-input type="text"></b-form-input>
                                    </b-col>
                                    <b-col sm="1">
                                        元
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-input type="text" placeholder="備註"></b-form-input>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>非配合場停車費</label>
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-input type="text"></b-form-input>
                                    </b-col>
                                    <b-col sm="1">
                                        元
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-input type="text" placeholder="備註"></b-form-input>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>拖吊費</label>
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-input type="text"></b-form-input>
                                    </b-col>
                                    <b-col sm="1">
                                        元
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-input type="text" placeholder="備註"></b-form-input>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>其他</label>
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-input type="text"></b-form-input>
                                    </b-col>
                                    <b-col sm="1">
                                        元
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-input type="text" placeholder="備註"></b-form-input>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>

                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="6" offset-sm="2">
                                        <b-button class="mr-2" variant="secondary" @click.prevent="inquire()">重製</b-button>
                                        <b-button class="mr-2" variant="primary" @click.prevent="inquire()">計算更改後結果</b-button>
                                        <b-button class="mr-2" variant="primary" @click.prevent="inquire()">送出處理</b-button>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                    </b-list-group>        
                </b-card>
            </div>
        </main>
        

    </div>
</template>

<script>
// @ is an alias to /src
import mainNav from '@/components/mainNav.vue'
import mainHeader from '@/components/mainHeader.vue'

export default {
    name: 'Home',
    components: {
        mainNav,
        mainHeader
    },
    data() {
        return {
            isHour12:false,
            selected: null,
            options: [
                { value: null, text: 'Please select an option' },
                { value: 'a', text: 'This is First option' },
                { value: 'b', text: 'Selected Option' },
                { value: { C: '3PO' }, text: 'This is an option with object value' },
                { value: 'd', text: 'This one is disabled', disabled: true }
            ],
            fields: [
                {key:'date', label:'申請日期'},
                {key:'name', label:'姓名'},
                {key:'id', label:'身分證字號'},
                {key:'gender', label:'性別'},
                {key:'distinguish', label:'處理區分'},
                {key:'final', label:'審核結果'},
                {key:'detail',label:'審核'}
            ],
            items: [
                { date: 40, name: 40, id: 'Dickerson', gender: 'Macdonald', distinguish:'處理區分',final:'結果',detail:'細節url' },
                { date: 'yyyy-mm-dd', name: 40, id: 'Dickerson', gender: 'Macdonald', distinguish:'處理區分',final:'結果',detail:'細節url' }
            ]
        }
    },
    methods: {
        clear(){
            console.log('clear')
        },
        inquire(){
            // if status = 200
            
        }
    },
}
</script>

<style scoped lang="scss">
.d-flex>.lable{
    flex:0 0 150px;
    line-height: calc(1.5em + 0.75rem + 2px);
}
label{
    margin: 0;
}

</style>
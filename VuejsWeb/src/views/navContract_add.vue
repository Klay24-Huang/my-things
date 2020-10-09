<template>
    <div>
        <mainHeader/>
        <!-- breadcum -->
        <div class="container text-left">
            <p class="text-secondary my-2">
                合約管理 / 
                <font-awesome-icon icon="file-alt"></font-awesome-icon> 人工新增預約
            </p>
        </div>
        <main class="bg-gray-400">
            <mainNav/>
            <div class="container mt-4 text-left">
                <b-card
                    header="人工新增預約"
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
                                        <label>會員帳號</label>
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-input type="text"></b-form-input>
                                    </b-col>
                                    <b-col sm="2">
                                        <b-button class="ml-2" variant="primary" @click.prevent="inquire()">確認帳號</b-button>
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
                                        <label>聯絡電話</label>
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
                                        <label>發票地址</label>
                                    </b-col>
                                    <b-col sm="6">
                                        <b-form-input type="text"></b-form-input>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>聯絡信箱</label>
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
                                        <label>優惠專案</label>
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
                                    <b-col sm="2">
                                        <label>取還車縣市</label>
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
                                    <b-col sm="2">
                                        <label>取還車地點</label>
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
                                    <b-col sm="2">
                                        <label>地址</label>
                                    </b-col>
                                    <b-col sm="6">
                                        {{address}}
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>地點說明</label>
                                    </b-col>
                                    <b-col sm="6">
                                        {{address}}
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>預約車型</label>
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
                                    <b-col sm="2">
                                        <label>取車時間</label>
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
                                        <label>還車時間</label>
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
                                        <label>發票寄送方式</label>
                                    </b-col>
                                    <b-col sm="6">
                                        <b-form-radio v-model="selected" name="some-radios" value="A">發票捐贈『家扶基金會』</b-form-radio>
                                        <b-form-radio v-model="selected" name="some-radios" value="B">二聯式發票以email寄送,中獎另行通知</b-form-radio>
                                        <b-form-radio v-model="selected" name="some-radios" value="C">郵寄二聯式</b-form-radio>
                                        <b-form-radio v-model="selected" name="some-radios" value="D">郵寄三聯式</b-form-radio>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>統一編號</label>
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
                                    <b-col sm="6" offset-sm="2">
                                        <b-button class="mr-2" variant="primary" @click.prevent="inquire()">確認送出</b-button>
                                        <b-button class="mr-2" variant="secondary" @click.prevent="inquire()">取消</b-button>
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
            address:'address',
            start:'',
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
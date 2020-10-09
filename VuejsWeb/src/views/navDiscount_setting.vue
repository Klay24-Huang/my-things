<template>
    <div>
        <mainHeader/>
        <!-- breadcum -->
        <div class="container text-left">
            <p class="text-secondary my-2">
                優惠管理 / 
                <font-awesome-icon icon="file-alt"></font-awesome-icon> APP優惠訊息推播
            </p>
        </div>
        <main class="bg-gray-400">
            <mainNav/>
            <div class="container text-left">
                <b-card
                    header="最新優惠"
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
                                        <label>類別</label>
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-group>
                                            <b-form-radio v-model="radio" name="some-radios" value="A">和運</b-form-radio>
                                            <b-form-radio v-model="radio" name="some-radios" value="B">系統</b-form-radio>
                                            <b-form-radio v-model="radio" name="some-radios" value="C">活動</b-form-radio>
                                            <b-form-radio v-model="radio" name="some-radios" value="D">優惠</b-form-radio>
                                        </b-form-group>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>標題</label>
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
                                        <label>發送類型</label>
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-group>
                                            <b-form-radio v-model="typeRadio" name="typeRadios" value="A">一般訊息</b-form-radio>
                                            <b-form-radio v-model="typeRadio" name="typeRadios" value="B">優惠edm</b-form-radio>
                                        </b-form-group>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>公布起訖日</label>
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-datepicker id="example-datepicker" placeholder="請選擇日期"></b-form-datepicker>
                                    </b-col>
                                    <b-col sm="3" class="text-center">
                                        <b-form-timepicker v-model="start" locale="en" :hour12="isHour12" class="ml-2"></b-form-timepicker>
                                    </b-col>
                                </b-row>
                                <b-row no-gutters>
                                    <b-col sm="3" offset-sm="2">
                                        <b-form-datepicker id="example-datepicker2" placeholder="請選擇日期"></b-form-datepicker>
                                    </b-col>
                                    <b-col sm="3" class="text-center">
                                        <b-form-timepicker v-model="start" locale="en" :hour12="isHour12" class="ml-2"></b-form-timepicker>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item>
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>內容</label>
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
                                        <b-button class="mr-2" variant="primary" @click.prevent="inquire()">確定新增</b-button>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                    </b-list-group>        
                </b-card>
            </div>
            <!-- output -->
            <div class="container mt-2">
                <b-table striped hover :items="items" :fields="fields" class="bg-white" head-row-variant="primary">
                    <template v-slot:cell(detail)="data">
                        
                        {{ data.item.detail }}
                    </template>
                </b-table>
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
            typeRadio:'',
            selected: null,
            radio:'',
            isHour12:false,
            start:'',
            options: [
                { value: null, text: 'Please select an option' },
                { value: 'a', text: 'This is First option' },
                { value: 'b', text: 'Selected Option' },
                { value: { C: '3PO' }, text: 'This is an option with object value' },
                { value: 'd', text: 'This one is disabled', disabled: true }
            ],
            fields: [
                {key:'date', label:'類型'},
                {key:'name', label:'標題'},
                {key:'id', label:'發送類型'},
                {key:'detail', label:'公布起訖日'},
                {key:'distinguish', label:'置頂'},,
                {key:'final',label:''}
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
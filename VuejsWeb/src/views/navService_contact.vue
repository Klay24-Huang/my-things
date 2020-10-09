<template>
    <div>
        <mainHeader/>
        <!-- breadcrum -->
        <div class="container text-left">
            <p class="text-secondary my-2">
                客服中心 / 
                <font-awesome-icon icon="file-alt"></font-awesome-icon> 合約/中控台
            </p>
        </div>
        <main class="bg-gray-400" >
            <mainNav/>
            <div class="container text-left" :class="{'d-none': isWatchingDetail}">
                <b-card
                    header="合約/中控台-查詢條件"
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
                                        <label>客戶帳號</label>
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
                                    <b-col sm="2">
                                        <label>合約號碼</label>
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
                                    <b-col sm="2">
                                        <label>車牌號碼</label>
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
                                        <b-button @click.prevent="clear()">清除</b-button>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                    </b-list-group>        
                </b-card>
            </div>
            <!-- output -->
            <div class="container mt-2" :class="{'d-none': isWatchingDetail}">
                <b-table striped hover :items="items" :fields="fields" class="bg-white" head-row-variant="primary">
                    <template v-slot:cell(detail)="data">
                        
                        {{ data.item.detail }}
                    </template>
                </b-table>
            </div>
            <!-- detail -->
            <div class="container mt-2" :class="{'d-none': !isWatchingDetail}">
                <b-table striped hover :items="detailItems" :fields="detailFields" class="bg-white" head-row-variant="primary">
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
            selected: null,
            options: [
                { value: null, text: 'Please select an option' },
                { value: 'a', text: 'This is First option' },
                { value: 'b', text: 'Selected Option' },
                { value: { C: '3PO' }, text: 'This is an option with object value' },
                { value: 'd', text: 'This one is disabled', disabled: true }
            ],
            fields: [
                {key:'contractId', label:'合約編號'},
                {key:'name', label:'會員帳號'},
                {key:'takeTimestamp', label:'預計取車時間'},
                {key:'returnTimestamp', label:'預計還車時間'},
                {key:'carNum', label:'車輛編號'},
                {key:'detail',label:'詳細資訊'}
            ],
            items: [
                { contractId: 40, name: 40, takeTimestamp: 'Dickerson', returnTimestamp: 'Macdonald', carNum:'處理區分',detail:'細節url' },
            ],
            //中控台
            isWatchingDetail: false,
            detailFields: [
                {key:'position', label:'據點'},
                {key:'carNum', label:'車號'},
                {key:'carStatus', label:'車機狀態'},
                {key:'returnTimestamp', label:'預計還車時間'},
                {key:'carNum', label:'車輛編號'},
                {key:'detail',label:'詳細資訊'}
            ],
            detailItems: [
                { position: 'iRent左營高鐵內站［二樓租賃車整備區］', carNum: 'RCR-6077', carStatus: [true,true,true], returnTimestamp: 'Macdonald', carNum:'處理區分',detail:'細節url' },
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
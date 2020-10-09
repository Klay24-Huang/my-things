<template>
    <div>
        <mainHeader/>
        <!-- breadcum -->
        <div class="container text-left">
            <p class="text-secondary my-2">
                報表 / 
                <font-awesome-icon icon="file-alt"></font-awesome-icon> 會員審核
            </p>
        </div>
        <main class="bg-gray-400">
            <mainNav/>
            <div class="container text-left">
                <b-card
                    header="綠界交易紀錄查詢"
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
                                        <label>營運狀態</label>
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-group>
                                            <b-form-radio-group
                                                id="radio-group-1"
                                                v-model="selected"
                                                :options="options"
                                                name="radio-options"
                                            ></b-form-radio-group>
                                        </b-form-group>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        
                        <b-list-group-item v-if="showCustomerId">
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>身分證字號</label>
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-input type="text" trim></b-form-input>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item v-if="showOrderId">
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>訂單編號</label>
                                    </b-col>
                                    <b-col sm="3">
                                        <b-form-input type="text" trim></b-form-input>
                                    </b-col>
                                </b-row>
                            </b-container>
                        </b-list-group-item>
                        <b-list-group-item v-if="showAuthorizeId">
                            <b-container fluid class="p-0">
                                <b-row class="my-1" no-gutters>
                                    <b-col sm="2">
                                        <label>授權單號</label>
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
            selected: 'a',
            options: [
                { value: 'a', text: '以客戶身分證字號為條件查詢' },
                { value: 'b', text: '以訂單為條件查詢' },
                { value: 'c', text: '以授權單號為條件查詢' }
            ],
            // table
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
    computed:{
        showCustomerId:function(){
            return this.selected == 'a'
        },
        showOrderId:function(){
            return this.selected == 'b'
        },
        showAuthorizeId:function(){
            return this.selected == 'c'
        },
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
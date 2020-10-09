<template>
    <div>
        <mainHeader/>
        <!-- breadcum -->
        <div class="container text-left">
            <p class="text-secondary my-2">
                合約管理 / 
                <font-awesome-icon icon="file-alt"></font-awesome-icon> 訂單紀錄歷程查詢
            </p>
        </div>
        <main class="bg-gray-400">
            <mainNav/>
            <div class="container text-left">
                <b-card
                    header="訂單紀錄歷程查詢"
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
                                        <label>訂單編號</label>
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
            <div class="container mt-4 d-none">
                <div class="d-flex justify-content-end mb-2">
                    <b-button class="mr-2" variant="primary">匯出Excel</b-button>
                    <b-button class="mr-2" variant="primary">匯出前一月Excel</b-button>
                    <b-button variant="primary">匯出當月Excel</b-button>
                </div>
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
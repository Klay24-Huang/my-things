<template>
    <div>
        <mainHeader/>
        <!-- breadcum -->
        <div class="container text-left">
            <p class="text-secondary my-2">
                據點管理 / 
                <font-awesome-icon icon="file-alt"></font-awesome-icon> 調度停車場
            </p>
        </div>
        <main class="bg-gray-400">
            <mainNav/>
            <div class="container text-left">
                <b-card
                    header="調度停車場"
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
                                    <label>處理項目</label>
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
                                    <label>停車場名稱</label>
                                </b-col>
                                <b-col sm="3">
                                    <b-form-input type="text" trim :disabled="isAdding"></b-form-input>
                                </b-col>
                            </b-row>
                        </b-container>
                    </b-list-group-item>
                    <b-list-group-item>
                        <b-container fluid class="p-0">
                            <b-row class="my-1" no-gutters>
                                <b-col sm="2">
                                    <label>匯入清單</label>
                                </b-col>
                                <b-col sm="3">
                                    <b-form-file :disabled="!isAdding" accept=".csv"></b-form-file>
                                </b-col>
                                <b-col sm="3">
                                    限定.csv檔
                                    <b-button class="mr-2" variant="primary" v-if="isAdding">範例檔案下載</b-button>
                                </b-col>
                            </b-row>
                        </b-container>
                    </b-list-group-item>
                    <b-list-group-item>
                        <b-container fluid class="p-0">
                            <b-row class="my-1" no-gutters>
                                <b-col sm="6" offset-sm="2">
                                    <b-button class="mr-2" variant="primary" v-if="isAdding">儲存</b-button>
                                    <b-button class="mr-2" variant="outline-secondary" v-if="isAdding">取消</b-button>
                                    <b-button class="mr-2" variant="primary" v-else @click="inquire()">查詢</b-button>
                                    <b-button class="mr-2" variant="outline-secondary" v-if="!isAdding">取消</b-button>
                                </b-col>
                            </b-row>
                        </b-container>
                    </b-list-group-item>
                </b-list-group>                
                </b-card>
            </div>
            <!-- output -->
            <div class="container text-left" :class="{ 'd-none': !isInquired}">
                <b-table striped hover :items="items" :fields="fields" class="bg-white mt-4" head-row-variant="primary" responsive="sm">
                    <template v-slot:cell(show_details)="row">
                        <b-button size="sm" @click="row.toggleDetails" class="mr-2" variant="link">
                        {{ row.detailsShowing ? '-' : '+'}}
                        </b-button>
                    </template>
                    <!-- 名稱 -->
                    <template v-slot:cell(name)="data">
                    <!-- `data.value` is the value after formatted by the Formatter -->
                        <b-form-input type="text" trim :placeholder="data.value"></b-form-input>
                    </template>
                    <!-- 地址 -->
                    <template v-slot:cell(address)="data">
                    <!-- `data.value` is the value after formatted by the Formatter -->
                        <b-form-input type="text" trim :placeholder="data.value"></b-form-input>
                    </template>
                    <template v-slot:cell(map)="data">
                    <!-- `data.value` is the value after formatted by the Formatter -->
                        <a @click="detailMap(data.value)">
                            <svg xmlns="http://www.w3.org/2000/svg" width="20.5" height="23.5" viewBox="0 0 41.045 46.614">
                                <path d="M622.622,642.891a1.371,1.371,0,0,0-1.185-.682h-24.53a1.369,1.369,0,0,0-1.184.682l-5.864,10.122a1.369,1.369,0,0,0,1.185,2.055H627.3a1.369,1.369,0,0,0,1.184-2.055Z" transform="translate(-588.649 -609.454)" fill="#fff" stroke="#d7000f" stroke-miterlimit="10" stroke-width="2"/>
                                <g transform="translate(6.135)">
                                    <path d="M620.231,616.77l-14.1,2.266-.023,0-.023,0-14.1-2.266s-.278,4.384,3.56,10.991c2.5,4.3,5.678,8.312,8.028,10.124a4.151,4.151,0,0,0,2.515.866l.023,0,.023,0a4.15,4.15,0,0,0,2.515-.866c2.35-1.812,5.531-5.825,8.028-10.124C620.51,621.154,620.231,616.77,620.231,616.77Z" transform="translate(-591.976 -602.642)" fill="#d7000f"/>
                                    <g transform="translate(0.004)">
                                    <ellipse cx="14.128" cy="14.128" rx="14.128" ry="14.128" fill="#d7000f"/>
                                    <ellipse cx="6.647" cy="6.647" rx="6.647" ry="6.647" transform="translate(7.481 7.481)" fill="#fff"/>
                                    </g>
                                </g>
                            </svg>
                        </a>
                    </template>
                    <template v-slot:cell(edit)="data">
                    <!-- `data.value` is the value after formatted by the Formatter -->
                        <b-button class="mr-2" variant="outline-secondary" @click="saveRow(data.value)">儲存</b-button>
                    </template>
                    <!-- detail -->
                    <template v-slot:row-details="row">
                        <b-card>
                            <b-row class="mb-2">
                                <b-col sm="2">經度</b-col>
                                <b-col sm="3">
                                    <b-form-input type="text" trim :placeholder="row.item.x"></b-form-input>
                                </b-col>
                            </b-row>

                            <b-row class="mb-2">
                                <b-col sm="2">緯度</b-col>
                                <b-col sm="3">
                                    <b-form-input type="text" trim :placeholder="row.item.y"></b-form-input>
                                </b-col>
                            </b-row>
                            <b-row class="mb-2">
                                <b-col sm="2">開放時間(起)</b-col>
                                <b-col sm="3">
                                    <b-form-input type="text" trim :placeholder="row.item.start"></b-form-input>
                                </b-col>
                            </b-row>
                            <b-row class="mb-2">
                                <b-col sm="2">開放時間(迄)</b-col>
                                <b-col sm="3">
                                    <b-form-input type="text" trim :placeholder="row.item.end"></b-form-input>
                                </b-col>
                            </b-row>
                        </b-card>
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
            selected: 'isAdding',
            isInquired:false,
            options: [
            { value: 'isAdding', text: '新增' },
            { value: 'isEditing', text: '修改' },
            ],
            // table
            fields: [
                {key:'index', label:''},
                {key:'show_details', label:''},
                {key:'name', label:'名稱'},
                {key:'address', label:'地址'},
                {key:'map', label:'地圖'},
                {key:'edit', label:''},
                // {key:'x', label:'經度'},
                // {key:'y', label:'緯度'},
                // {key:'space', label:'車位數'},
                // {key:'online', label:'實際上線'},
            ],
            items: [ 
            { index:1,isEditing: false, age: 40, name: 'Dickerson', address: 'Macdonald',map: 'map',edit: 'edit',x:0.0,y:1.1,start:'yyyy-mm-dd',end:'yyyy-mm-dd' },
            { index:2,isEditing: false, age: 21, name: 'Larsen', address: 'Shaw',map: 'map',edit: 'edit',x:0.0,y:1.1,start:'yyyy-mm-dd',end:'yyyy-mm-dd' },
            {
                index:3,
                isEditing: false,
                age: 89,
                name: 'Geneva',
                address: 'Wilson',
                map: 'map',edit: 'edit',x:0.0,y:1.1,start:'yyyy-mm-dd',end:'yyyy-mm-dd'
            },
            { 
                index:4,
                isEditing: true, age: 38, name: 'Jami', address: 'Carney',map: 'map',edit: 'edit',x:0.0,y:1.1,start:'yyyy-mm-dd',end:'yyyy-mm-dd' }
            ]
        }
    },
    computed: {
        isAdding(){
            return this.selected == 'isAdding'
        }
    },
    methods: {
        inquire(){
            this.isInquired = true
        },
        saveRow(val){
            console.log(val)
            // call api
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
    line-height: calc(1.5em + 0.75rem + 2px);
    margin: 0;
}

</style>
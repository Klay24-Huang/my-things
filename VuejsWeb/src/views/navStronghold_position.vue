<template>
    <div>
        <mainHeader/>
        <!-- breadcum -->
        <div class="container text-left">
            <p class="text-secondary my-2">
                據點管理 / 
                <font-awesome-icon icon="file-alt"></font-awesome-icon> 據點資訊設定
            </p>
        </div>
        <main class="bg-gray-400">
            <mainNav/>
            <div class="container text-left">
                <b-card
                    header="據點資訊設定"
                    header-text-variant="white"
                    header-bg-variant="secondary"
                    class="mt-4"
                    v-if="!isCreating && !isEditing"
                    no-body
                >
                    
                <b-list-group flush>
                    <b-list-group-item>
                        <b-container fluid class="p-0">
                            <b-row class="my-1" >
                                <b-col sm="2">
                                    <label>服務據點</label>
                                </b-col>
                                <b-col sm="3">
                                    <b-form-input type="text" trim></b-form-input>
                                </b-col>
                            </b-row>
                        </b-container>
                    </b-list-group-item>
                    <b-list-group-item>
                        <b-container fluid class="p-0">
                            <b-row class="my-1" >
                                <b-col sm="2">
                                    <label>篩選功能</label>
                                </b-col>
                                <b-col sm="3">
                                    <b-form-checkbox
                                        id="checkbox-1"
                                        v-model="status"
                                        name="checkbox-1"
                                        value="true"
                                        unchecked-value="false"
                                    >
                                        車位數與上線數不相等
                                    </b-form-checkbox>
                                </b-col>
                            </b-row>
                        </b-container>
                    </b-list-group-item>
                    <b-list-group-item>
                        <b-container fluid class="p-0">
                            <b-row class="my-1" >
                                <b-col sm="6" offset-sm="2">
                                    <b-button class="mr-2" variant="primary" @click="inquire()">查詢</b-button>
                                    <b-button class="mr-2" variant="info" @click="switchCreat()">新增</b-button>
                                </b-col>
                            </b-row>
                        </b-container>
                    </b-list-group-item>
                </b-list-group>                
                </b-card>
                <navStronghold_position_creat
                v-if="this.isCreating" 
                @update="creatingUpdate" 
                @save="saveUpdate"></navStronghold_position_creat>
                <navStronghold_position_edit
                v-if="this.isEditing" 
                @update="editingUpdate"
                @save="saveeditUpdate"></navStronghold_position_edit>
            </div>
            <!-- output -->
            <div class="container mt-2" v-if="isInquire">
                <b-table striped hover :items="items" :fields="fields" class="bg-white" head-row-variant="primary">
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
                        <b-button class="mr-2" variant="outline-secondary" @click="detailEdit(data.value)">修改</b-button>
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
import navStronghold_position_edit from '@/components/navStronghold_position_edit.vue'
import navStronghold_position_creat from '@/components/navStronghold_position_creat.vue'

export default {
    name: 'Home',
    components: {
        mainNav,
        mainHeader,
        navStronghold_position_edit,
        navStronghold_position_creat
    },
    data() {
        return {
            status:'',
            isInquire:false,
            isCreating:false,
            isEditing:false,
            selected: null,
            options: [
                { value: null, text: 'Please select an option' },
            { value: 'a', text: 'This is First option' },
            { value: 'b', text: 'Selected Option' },
            { value: { C: '3PO' }, text: 'This is an option with object value' },
            { value: 'd', text: 'This one is disabled', disabled: true }
            ],
            //data for table
            fields: [
                {key:'index', label:''},
                {key:'id', label:'ID'},
                {key:'name', label:'名稱'},
                {key:'address', label:'地址'},
                {key:'x', label:'經度'},
                {key:'y', label:'緯度'},
                {key:'map', label:'地圖'},
                {key:'space', label:'車位數'},
                {key:'online', label:'實際上線'},
                {key:'edit', label:''},
            ],
            items: [
                { index: 1, name: 'irent站點名稱', id: 'X0A0', address: '高雄市美術館東路', x:'0.0000',y:'1.111111',map:'detailmap data',space:'00',online:'100',edit:'editdata' },
                { index: 2, name: 'irent站點名稱', id: 'X0A0', address: '高雄市美術館東路', x:'2.0000',y:'2.111111',map:'detailmap data',space:'00',online:'100',edit:'editdata' },
            ]
        }
    },
    computed: {
        // 直接当做普通属性调用不加括号
        // 任何data中数据变化立即重新计算
        // 计算属性会缓存
        editingPicfile:{
            get(){
                return `pic` + this.picEditing + `file`
            },
            set(){
                return `pic` + this.picEditing + `file`
            }
        }
    },
    methods: {
        inquire(){
            //call api to inquire data
            this.isInquire = true
        },
        saveCreat(){
            //call api to post new data
        },
        switchCreat(){
            this.isCreating = true
            this.isInquire = false
        },
        creatingUpdate(val){
            this.isCreating = val
        },
        saveUpdate(val){
            this.isCreating = val
        },
        saveeditUpdate(val){
            this.isEditing = val
        },
        editingUpdate(val){
            this.isEditing = val
        },
        //table
        detailMap(val){
            console.log(val)
            //call api
        },
        detailEdit(val){
            console.log(val)
            //emmit in data
            this.isInquire = false
            this.isEditing = true
        },
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
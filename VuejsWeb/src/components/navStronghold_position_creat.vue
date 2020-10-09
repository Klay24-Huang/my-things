<template>
    <div >
        <b-card
        header="據點新增"
        header-text-variant="white"
        header-bg-variant="secondary"
        class="mt-4"
        no-body>
            <b-list-group-item>
                <b-container fluid class="p-0">
                    <b-row class="my-1" >
                        <b-col sm="2">
                            <label>據點類別</label>
                        </b-col>
                        <b-col sm="3">
                            <b-form-select v-model="selected" :options="options"></b-form-select>
                        </b-col>
                    </b-row>
                </b-container>
            </b-list-group-item>
            <b-list-group-item>
                <b-container fluid class="p-0">
                    <b-row class="my-1" >
                        <b-col sm="2">
                            <label>據點代碼</label>
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
                            <label>據點名稱</label>
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
                            <label>統一編號</label>
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
                                    <label>管轄據點</label>
                                </b-col>
                                <b-col sm="3">
                                    <b-form-select v-model="selected" :options="options"></b-form-select>
                                </b-col>
                                <b-col sm="3">
                                    <span>X011~X093</span>
                                </b-col>
                            </b-row>
                        </b-container>
                    </b-list-group-item>
                    <b-list-group-item>
                        <b-container fluid class="p-0">
                            <b-row class="my-1" >
                                <b-col sm="2">
                                    <label>郵遞區號</label>
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
                                    <label>據點地址</label>
                                </b-col>
                                <b-col sm="3">
                                    <b-form-select v-model="selected" :options="options" plcahoder="縣市"></b-form-select>
                                </b-col>
                                <b-col sm="3">
                                    <b-form-select v-model="selected" :options="options" plcahoder="區"></b-form-select>
                                </b-col>
                                <b-col sm="4">
                                    <b-form-input type="text" trim></b-form-input>
                                </b-col>
                            </b-row>
                        </b-container>
                    </b-list-group-item>
                    <b-list-group-item>
                        <b-container fluid class="p-0">
                            <b-row class="my-1" >
                                <b-col sm="2">
                                    <label>聯絡電話</label>
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
                                    <label>經度</label>
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
                                    <label>緯度</label>
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
                                    <label>據點描述(內部註記)</label>
                                </b-col>
                                <b-col sm="3">
                                    <b-form-textarea
                                    id="textarea"
                                    v-model="text"
                                    rows="3"
                                    max-rows="6"
                                    ></b-form-textarea>
                                </b-col>
                            </b-row>
                        </b-container>
                    </b-list-group-item>
                    <b-list-group-item>
                        <b-container fluid class="p-0">
                            <b-row class="my-1" >
                                <b-col sm="2">
                                    <label>據點描述(App顯示)</label>
                                </b-col>
                                <b-col sm="3">
                                    <b-form-textarea
                                    id="textarea"
                                    v-model="text"
                                    rows="3"
                                    max-rows="6"
                                    ></b-form-textarea>
                                </b-col>
                            </b-row>
                        </b-container>
                    </b-list-group-item>
                    <b-list-group-item>
                        <b-container fluid class="p-0">
                            <b-row class="my-1" >
                                <b-col sm="2">
                                    <label>據點說明照片上傳</label>
                                </b-col>
                                <b-col sm="6">
                                    <b-container fluid>
                                        <b-row no-gutters>
                                            <b-col :order="picAorder">
                                                <label for="picA" class="pic_thumbnail" :class="{'active':this.picEditing==='A'}"></label>
                                                <b-form-radio id="picA" v-model="picEditing" class="d-none" name="picEditing" value="A"></b-form-radio>
                                            </b-col>
                                            <b-col :order="picBorder">
                                                <label for="picB" class="pic_thumbnail" :class="{'active':this.picEditing==='B'}"></label>
                                                <b-form-radio id="picB" v-model="picEditing" class="d-none" name="picEditing" value="B"></b-form-radio>
                                            </b-col>
                                            <b-col :order="picCorder">
                                                <label for="picC" class="pic_thumbnail" :class="{'active':this.picEditing==='C'}"></label>
                                                <b-form-radio id="picC" v-model="picEditing" class="d-none" name="picEditing" value="C"></b-form-radio>
                                            </b-col>
                                            <b-col :order="picDorder">
                                                <label for="picD" class="pic_thumbnail" :class="{'active':this.picEditing==='D'}"></label>
                                                <b-form-radio id="picD" v-model="picEditing" class="d-none" name="picEditing" value="D"></b-form-radio>
                                            </b-col>
                                            <b-col :order="picEorder">
                                                <label for="picE" class="pic_thumbnail" :class="{'active':this.picEditing==='E'}"></label>
                                                <b-form-radio id="picE" v-model="picEditing" class="d-none" name="picEditing" value="E"></b-form-radio>
                                            </b-col>
                                        </b-row>
                                        <b-row>
                                            <b-form-group
                                                label="排列順序: "
                                                label-for="picAorder"
                                                v-if="this.picEditing==='A'"
                                            >
                                                <b-form-select id="picAorder" v-model="picAorder" :options="picOrders"></b-form-select>
                                            </b-form-group>
                                            <b-form-group
                                                label="排列順序: "
                                                label-for="picBorder"
                                                v-if="this.picEditing==='B'"
                                            >
                                                <b-form-select id="picBorder" v-model="picBorder" :options="picOrders"></b-form-select>
                                            </b-form-group>
                                            <b-form-group
                                                label="排列順序: "
                                                label-for="picCorder"
                                                v-if="this.picEditing==='C'"
                                            >
                                                <b-form-select id="picCorder" v-model="picCorder" :options="picOrders"></b-form-select>
                                            </b-form-group>
                                            <b-form-group
                                                label="排列順序: "
                                                label-for="picDorder"
                                                v-if="this.picEditing==='D'"
                                            >
                                                <b-form-select id="picDorder" v-model="picDorder" :options="picOrders"></b-form-select>
                                            </b-form-group>
                                            <b-form-group
                                                label="排列順序: "
                                                label-for="picEorder"
                                                v-if="this.picEditing==='E'"
                                            >
                                                <b-form-select id="picEorder" v-model="picEorder" :options="picOrders"></b-form-select>
                                            </b-form-group>
                                        </b-row>
                                        <b-row>
                                            <b-form-group
                                                label="照片說明: "
                                                label-for="picAinfo"
                                                v-if="this.picEditing==='A'"
                                            >
                                                <b-form-input id="picAinfo" v-model="picAinfo" key="this.picEditing"></b-form-input>
                                            </b-form-group>
                                            <b-form-group
                                                label="照片說明: "
                                                label-for="picBinfo"
                                                v-if="this.picEditing==='B'"
                                            >
                                                <b-form-input id="picBinfo" v-model="picBinfo" key="this.picEditing"></b-form-input>
                                            </b-form-group>
                                            <b-form-group
                                                label="照片說明: "
                                                label-for="picCinfo"
                                                v-if="this.picEditing==='C'"
                                            >
                                                <b-form-input id="picCinfo" v-model="picCinfo" key="this.picEditing"></b-form-input>
                                            </b-form-group>
                                            <b-form-group
                                                label="照片說明: "
                                                label-for="picDinfo"
                                                v-if="this.picEditing==='D'"
                                            >
                                                <b-form-input id="picDinfo" v-model="picDinfo" key="this.picEditing"></b-form-input></b-form-input>
                                            </b-form-group>
                                            <b-form-group
                                                label="照片說明: "
                                                label-for="picEinfo"
                                                v-if="this.picEditing==='E'"
                                            >
                                                <b-form-input id="picEinfo" v-model="picEinfo" key="this.picEditing"></b-form-input>
                                            </b-form-group>
                                        </b-row>
                                        <b-row>
                                            <b-form-group
                                                label="上傳照片: "
                                                label-for="picfile"
                                                description="限100kb以內"
                                                v-if="this.picEditing==='A'"
                                            >
                                                <b-form-file 
                                                id="picfile" 
                                                multiple 
                                                accept="image/*" 
                                                class="text-left" 
                                                v-model="picfile"
                                                v-if="this.picEditing==='A'"
                                                ></b-form-file>
                                            </b-form-group>
                                        </b-row>
                                    </b-container>
                                </b-col>
                            </b-row>
                        </b-container>
                    </b-list-group-item>
                    <b-list-group-item>
                        <b-container fluid class="p-0">
                            <b-row class="my-1" >
                                <b-col sm="2">
                                    <label>還車位置資訊<br>是否必填</label>
                                </b-col>
                                <b-col sm="3">
                                    <b-form-radio v-model="selected" name="some-radios" value="A">是</b-form-radio>
                                    <b-form-radio v-model="selected" name="some-radios" value="B">否</b-form-radio>
                                </b-col>
                            </b-row>
                        </b-container>
                    </b-list-group-item>
                    <b-list-group-item>
                        <b-container fluid class="p-0">
                            <b-row class="my-1" >
                                <b-col sm="2">
                                    <label>共同出車庫位</label>
                                </b-col>
                                <b-col sm="3">
                                    <b-form-select v-model="selected" :options="options"></b-form-select>
                                </b-col>
                            </b-row>
                        </b-container>
                    </b-list-group-item>
                    <b-list-group-item>
                        <b-container fluid class="p-0">
                            <b-row class="my-1" >
                                <b-col sm="2">
                                    <label>財務-部門代碼</label>
                                </b-col>
                                <b-col sm="6">
                                    <b-form-input type="text" trim></b-form-input>
                                </b-col>
                            </b-row>
                        </b-container>
                    </b-list-group-item>
                    <b-list-group-item>
                        <b-container fluid class="p-0">
                            <b-row class="my-1" >
                                <b-col sm="2">
                                    <label>有效起日</label>
                                </b-col>
                                <b-col sm="3">
                                    <b-form-datepicker v-model="begin" ></b-form-datepicker>
                                </b-col>
                            </b-row>
                        </b-container>
                    </b-list-group-item>
                    <b-list-group-item>
                        <b-container fluid class="p-0">
                            <b-row class="my-1" >
                                <b-col sm="2">
                                    <label>有效迄日</label>
                                </b-col>
                                <b-col sm="3">
                                    <b-form-datepicker v-model="end"></b-form-datepicker>
                                </b-col>
                            </b-row>
                        </b-container>
                    </b-list-group-item>
                    <b-list-group-item>
                        <b-container fluid class="p-0">
                            <b-row class="my-1" >
                                <b-col sm="2">
                                    <label>電子柵欄</label>
                                </b-col>
                                <b-col sm="3">
                                    <b-button class="mr-2" variant="primary" @click="mapFunction()">MAP</b-button>
                                </b-col>
                            </b-row>
                        </b-container>
                    </b-list-group-item>
                    <b-list-group-item>
                        <b-container fluid class="p-0">
                            <b-row class="my-1" >
                                <b-col sm="2">
                                    <label>車位數</label>
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
                                    <label>實際上線數</label>
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
                                <b-col sm="3" offset-sm="2">
                                    <b-button class="mr-2" variant="primary" @click.prevent="saveCreat()">儲存</b-button>
                                    <b-button @click.prevent="cancelCreat()">取消</b-button>
                                </b-col>
                            </b-row>
                        </b-container>
                    </b-list-group-item>
        </b-card>
    </div>
</template>

<script>
export default {
    name: 'navStronghold_position_creat',
    props: {
        msg: String
    },
    data() {
        return {
            isCreating:true,
            selected:'',
            text:'text',
            begin:'',
            end:'',
            options: [
                { value: null, text: 'Please select an option' },
            { value: 'a', text: 'This is First option' },
            { value: 'b', text: 'Selected Option' },
            { value: { C: '3PO' }, text: 'This is an option with object value' },
            { value: 'd', text: 'This one is disabled', disabled: true }
            ],
            //available options for pic order
            picOrders:[
                {value:'1',text:'1'},
                {value:'2',text:'2'},
                {value:'3',text:'3'},
                {value:'4',text:'4'},
                {value:'5',text:'5'},
            ],
            picEditing:'A',
            //pic a
            picAorder:'1',
            picAinfo:'',
            //pic b
            picBorder:'2',
            picBinfo:'',
            //pic c
            picCorder:'3',
            picCinfo:'',
            //pic d
            picDorder:'4',
            picDinfo:'',
            //pic e
            picEorder:'5',
            picEinfo:'',
            //files
            picfile:[],
        }
    },
    methods: {
        cancelCreat(){
            //reset value
            //emit
            this.isCreating = false
            this.$emit('update',this.isCreating)
        },
        saveCreat(){
            //call api,then
            this.isCreating = false
            this.$emit('save',this.isCreating)
        },
        mapFunction(){
            // someting here for map
        }
    },
}
</script>
<style lang="scss" scoped>
.pic_thumbnail{
    border: 1px solid #aaa;
    width: calc(1.5em + 0.75rem + 2px);
    height: calc(1.5em + 0.75rem + 2px);
}
.pic_thumbnail.active{
    border: 1px solid red;
}
</style>
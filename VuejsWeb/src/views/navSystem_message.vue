<template>
    <div>
        <mainHeader/>
        <!-- breadcum -->
        <div class="container text-left">
            <p class="text-secondary my-2">
                系統管理 / 
                <font-awesome-icon icon="file-alt"></font-awesome-icon> 訊息記錄查詢
            </p>
        </div>
        <main class="bg-gray-400">
            <mainNav/>
            <div class="container text-left">
                <b-card
                    header="訊息記錄查詢"
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
                                    <label>發送日期</label>
                                </b-col>
                                <b-col sm="3">
                                    <b-form-datepicker id="example-datepicker" placeholder="請選擇日期"></b-form-datepicker>
                                </b-col>
                            </b-row>
                        </b-container>
                    </b-list-group-item>
                    <b-list-group-item>
                        <b-container fluid class="p-0">
                            <b-row class="my-1" no-gutters>
                                <b-col sm="2">
                                    <label>車號</label>
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
                                    <label>執行功能</label>
                                </b-col>
                                <b-col sm="4">
                                    <b-form-radio-group
                                        id="radio-group-1"
                                        v-model="selected"
                                        :options="options"
                                        name="radio-options"
                                    ></b-form-radio-group>
                                </b-col>
                            </b-row>
                        </b-container>
                    </b-list-group-item>
                    
                    <b-list-group-item>
                        <b-container fluid class="p-0">
                            <b-row class="my-1" no-gutters>
                                <b-col sm="6" offset-sm="2">
                                    <b-button class="mr-2" variant="primary">查詢</b-button>
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
        mainHeade
    },
    data() {
        return {
            selected: null,
            options: [
            { value: 'a', text: '卡號設定紀錄(to 車機)' },
            { value: 'b', text: '刷卡紀錄(from 車用讀卡機)' },
            { value: 'd', text: '車輛事件狀態'}
            ]
        }
    }
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
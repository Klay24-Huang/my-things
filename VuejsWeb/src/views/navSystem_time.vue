<template>
    <div>
        <mainHeader/>
        <!-- breadcum -->
        <div class="container text-left">
            <p class="text-secondary my-2">
                系統管理 / 
                <font-awesome-icon icon="file-alt"></font-awesome-icon> 車輛管理時間查詢
            </p>
        </div>
        <main class="bg-gray-400">
            <mainNav/>
            <div class="container text-left">
                <b-card
                    header="車輛管理時間查詢"
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
                                    <label>據點</label>
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
                                    <label>日期</label>
                                </b-col>
                                <b-col sm="3">
                                    <b-form-datepicker id="example-datepicker" placeholder="請選擇日期"></b-form-datepicker>
                                </b-col>
                                <b-col sm="1" class="text-center">
                                    ~
                                </b-col>
                                <b-col sm="3" class="text-center">
                                    <b-form-datepicker id="example-datepicker2" placeholder="請選擇日期"></b-form-datepicker>
                                </b-col>
                            </b-row>
                        </b-container>
                    </b-list-group-item>
                    
                    <b-list-group-item>
                        <b-container fluid class="p-0">
                            <b-row class="my-1" no-gutters>
                                <b-col sm="6" offset-sm="2">
                                    <b-button class="mr-2" variant="primary">確定送出</b-button>
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
            selected: null,
            options: [
            { value: 'a', text: '預約' },
            { value: 'b', text: '取車' },
            { value: 'd', text: '還車'}
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
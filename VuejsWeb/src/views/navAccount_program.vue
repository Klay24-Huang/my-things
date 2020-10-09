<template>
    <div>
        <mainHeader/>
        <!-- breadcum -->
        <div class="container text-left">
            <p class="text-secondary my-2">
                帳號管理 / 
                <font-awesome-icon icon="file-alt"></font-awesome-icon> 功能程式維護
            </p>
        </div>
        <main class="bg-gray-400">
            <mainNav/>
            <div class="container text-left">
                <b-card
                    header="功能程式維護"
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
                                    <label>處理區分</label>
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
                                    <label>功能群組名稱</label>
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
                                    <label>功能編號(ID)</label>
                                </b-col>
                                <b-col sm="3">
                                    <b-form-input type="text" trim></b-form-input>
                                </b-col>
                                <b-col sm="2">
                                    限英數字
                                </b-col>
                            </b-row>
                        </b-container>
                    </b-list-group-item>
                    <b-list-group-item>
                        <b-container fluid class="p-0">
                            <b-row class="my-1" no-gutters>
                                <b-col sm="2">
                                    <label>功能名稱</label>
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
                                    <b-button class="mr-2" variant="primary">存檔</b-button>
                                    <b-button>取消</b-button>
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
            { value: null, text: 'Please select an option' },
            { value: 'a', text: 'This is First option' },
            { value: 'b', text: 'Selected Option' },
            { value: { C: '3PO' }, text: 'This is an option with object value' },
            { value: 'd', text: 'This one is disabled', disabled: true }
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
    margin: 0;
}

</style>
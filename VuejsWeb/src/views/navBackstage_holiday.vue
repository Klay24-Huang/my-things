<template>
    <div>
        <mainHeader/>
        <!-- breadcum -->
        <div class="container text-left">
            <p class="text-secondary my-2">
                後臺管理 / 
                <font-awesome-icon icon="file-alt"></font-awesome-icon> 平假日維護
            </p>
        </div>
        <main class="bg-gray-400">
            <mainNav/>
            <div class="container text-left">
                <b-card
                    header="平假日維護"
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
                                    <label>平假日更新檔</label>
                                </b-col>
                                <b-col sm="3">
                                    <b-form-file></b-form-file>
                                </b-col>
                                <b-col sm="2"> 
                                    <b-button class="ml-2" variant="primary">下載現行檔</b-button>
                                </b-col>
                            </b-row>
                        </b-container>
                    </b-list-group-item>
                    <b-list-group-item>
                        <b-container>
                            <b-row no-gutters>
                                <b-col offset-sm="2">
                                    <span class="text-danger">若要更新，則請先下載現行檔案進行更改，更改後再行上傳</span>
                                </b-col>
                            </b-row>
                        </b-container>
                    </b-list-group-item>
                    <b-list-group-item>
                        <b-container fluid class="p-0">
                            <b-row class="my-1" no-gutters>
                                <b-col sm="6" offset-sm="2">
                                    <b-button class="mr-2" variant="primary">確認送出</b-button>
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
        mainHead
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
    line-height: calc(1.5em + 0.75rem + 2px);
    margin: 0;
}

</style>
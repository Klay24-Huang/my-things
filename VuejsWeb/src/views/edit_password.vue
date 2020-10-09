<template>
    <div>
        <mainHeader/>
        <!-- breadcum -->
        <div class="container text-left">
            <p class="text-secondary my-2">
                修改密碼
            </p>
        </div>
        <main class="bg-gray-400">
            <mainNav/>
            <div class="container text-left">
                <b-card
                    header="修改密碼"
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
                                    <label>舊密碼</label>
                                </b-col>
                                <b-col sm="3">
                                    <b-form-input type="password" trim></b-form-input>
                                </b-col>
                            </b-row>
                        </b-container>
                    </b-list-group-item>
                    <b-list-group-item>
                        <b-container fluid class="p-0">
                            <b-row class="my-1" no-gutters>
                                <b-col sm="2">
                                    <label>新密碼</label>
                                </b-col>
                                <b-col sm="3">
                                    <b-form-input type="password" trim></b-form-input>
                                </b-col>
                            </b-row>
                        </b-container>
                    </b-list-group-item>
                    <b-list-group-item>
                        <b-container fluid class="p-0">
                            <b-row class="my-1" no-gutters>
                                <b-col sm="2">
                                    <label>確認新密碼</label>
                                </b-col>
                                <b-col sm="3">
                                    <b-form-input type="password" trim></b-form-input>
                                </b-col>
                            </b-row>
                        </b-container>
                    </b-list-group-item>
                    
                    <b-list-group-item>
                        <b-container fluid class="p-0">
                            <b-row class="my-1" no-gutters>
                                <b-col sm="6" offset-sm="2">
                                    <b-button class="mr-2" variant="primary" @click="updatePassword()">確認更改</b-button>
                                    <b-button class="mr-2" variant="outline-secondary">清除</b-button></b-button>
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
    },
    methods: {
        updatePassword(){
            // api
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
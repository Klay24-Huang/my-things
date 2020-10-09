<!-- header -->
<template>
    <div class="header bg-dark text-light">
        <div class="container justify-content-between d-flex text-left">
            iRent無人化租車租車共享系統
            <div>
                <b-button variant="link" class="text-white">
                    <svg xmlns="http://www.w3.org/2000/svg" width="14.142" height="14.999" viewBox="0 0 14.142 14.999">
                        <g id="power" transform="translate(0)">
                            <path id="Path_9" data-name="Path 9" d="M515.538,687.48h0a.815.815,0,0,0,.262,1.028,5.421,5.421,0,1,1-6.244,0,.816.816,0,0,0,.261-1.033h0a.832.832,0,0,0-1.228-.3,7.009,7.009,0,0,0-2.973,6.1,7.072,7.072,0,0,0,14.133-.361,7.011,7.011,0,0,0-3.006-5.753A.824.824,0,0,0,515.538,687.48Z" transform="translate(-505.606 -684.946)" fill="#fff"/>
                            <rect id="Rectangle_61" data-name="Rectangle 61" width="1.65" height="4.22" rx="0.825" transform="translate(6.246)" fill="#fff"/>
                        </g>
                    </svg>
                    登出
                </b-button>
                |
                <b-button variant="link" class="text-white"
                to="/edit_password">
                    修改密碼
                </b-button>            
            </div>
        </div>
    </div>
</template>

<script>
export default {
    name: 'mainHeader',
    props: {
        msg: String
    }
}
</script>
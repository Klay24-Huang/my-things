<template>
    <div class="home">
        <img alt="Vue logo" src="../assets/logo.png" class="d-none">
        <mainNav/>
    </div>
</template>

<script>
// @ is an alias to /src
import mainNav from '@/components/mainNav.vue'

export default {
    name: 'Home',
    components: {
        mainNav
    }
}
</script>


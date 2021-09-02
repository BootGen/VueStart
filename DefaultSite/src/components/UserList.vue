<template>
  <div class="col-xxl-3 col-md-6 col-sm-12">
    <div class="m-2" v-for="item in modelValue" :key="item.id">
      <user-edit v-model="item.value" @close="finishEditing()" v-if="editedUserId === item.id"></user-edit>
      <user-view v-model="item.value" @delete="deleteItem(item)" @edit="editItem(item)" v-else></user-view>
    </div>
  </div>
</template>


<script>
import { defineComponent, ref } from 'vue';
import UserView from '../components/UserView.vue';
import UserEdit from '../components/UserEdit.vue';

export default defineComponent({
  components: { UserView, UserEdit },
  name: 'UserList',
  props: {
    modelValue: Object
  },
  setup() {
    const editedUserId = ref(-1)
    return { editedUserId }
  },
  methods: {
    editItem(item){
      this.editedUserId = item.id;
    },
    deleteItem(item){
      if (confirm(`Are you sure to delete User with name ${ item.value.userName }?`)){
        this.userList = this.userList.filter(i => i.id !== item.id);
        this.$emit('update:modelValue', this.userList.map(i => i.value))
      }
    },
    finishEditing() {
      this.editedUserId = -1;
    }
  }
});
</script>
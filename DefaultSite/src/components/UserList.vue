<template>
  <div class="col-xxl-3 col-md-6 col-sm-12">
    <div class="m-2" v-for="item in modelValue" :key="item.id">
      <user-edit v-model="item.value" @canceled="finishEditing()" @saved="finishEditing()" v-if="editedUserId === item.id"></user-edit>
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
  setup(props, context) {
    const editedUserId = ref(-1);
    const editItem = function(item){
      editedUserId.value = item.id;
    };
    
    const deleteItem = function(item){
      if (confirm(`Are you sure to delete this user?`)) {
        console.log(props.modelValue)
        context.emit('update:modelValue', props.modelValue.filter(i => i.id !== item.id))
      }
    };
    
    const finishEditing = function() {
      editedUserId.value = -1;
    };

    return { editedUserId, editItem, deleteItem, finishEditing }
  }
});
</script>
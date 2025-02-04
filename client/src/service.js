import axios from 'axios';

const API_URL = process.env.REACT_APP_API_URL;
const api = axios.create({
  baseURL: API_URL, // הגדרת כתובת הבסיס כברירת מחדל
  headers: { "Content-Type": "application/json" }
});

// Interceptor לתפיסת שגיאות בתשובות השרת
api.interceptors.response.use(
  
  response => response, // מחזיר את התשובה כרגיל אם הכל תקין
  error => {
  console.error("📌 API Error:", error.response?.status, error.response?.data);

    if (error.response) {
      switch (error.response.status) {
        case 400:
          console.error("⚠️ בקשה שגויה (Bad Request)");
          break;
        case 401:
          console.error("🔒 אין הרשאה (Unauthorized)");
          alert("אין לך הרשאה, נא להתחבר מחדש.");
          break;
        case 500:
          console.error("🔥 שגיאת שרת (Internal Server Error)");
          alert("שגיאת שרת! נסה שוב מאוחר יותר.");
          break;
        default:
          console.log("i am tehila");
          console.error("❌ שגיאה לא ידועה:", error.response.status);
      }
    } else {
      console.error("❌ שגיאה ללא תגובה    מהשרת:", error.message);
    }

    return Promise.reject(error); // מחזיר את השגיאה להמשך טיפול
  }
);

export default {
  getTasks: async () => {
    const result = await api.get("/items");
    return result.data;
  },

  addTask: async (name) => {
    console.log('add task', name);
  
    const newTask = { name, isComplete: false };
  
    try {
      const result = await api.post("/items", newTask, {
        headers: { "X-Request-ID": "12345" }
      });
      return result.data;
    } catch (error) {
      console.error("שגיאה בהוספת משימה:", error);
      throw error; // זורקים שגיאה כדי שמי שקורא לפונקציה יוכל לתפוס אותה
    }
  },

  setCompleted: async (id, isComplete) => {
    console.log('setCompleted', { id, isComplete });
    const updatedTask = { isComplete};
    await api.put(`/items/${id}`, updatedTask);
    return { id, isComplete };
  },

  deleteTask: async (id) => {
    console.log('delete task', id);
    await api.delete(`/items/${id}`);
    return { success: true, id };
  }
};

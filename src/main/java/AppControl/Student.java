package AppControl;

public class Student {
    private String studentID;
    private String name;
    private String nationality;

    public Student(String input) {
        String[] data = input.split("_");
        studentID = data[0];
        name = data[1];
        nationality = data[2];
    }

    public String getStudentID() {
        return studentID;
    }

    public String getName() {
        return name;
    }

    public String getNationality() {
        return nationality;
    }

    public void setStudentID(String studentID) {
        this.studentID = studentID;
    }
}

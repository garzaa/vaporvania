public class Alert {
	public readonly string content;
	public readonly bool priority;

	public Alert(string content) {
		this.content = content;
	}

	public Alert(string content, bool priority) {
		this.content = content;
		this.priority = priority;
	}
}

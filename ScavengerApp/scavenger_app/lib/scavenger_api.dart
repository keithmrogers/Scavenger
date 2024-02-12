import 'dart:convert';
import 'package:http/http.dart' as http;

class ScavengerAPI {
  final String baseUrl;

  ScavengerAPI() : baseUrl = const String.fromEnvironment("API_URL");

  Future<Map<String, dynamic>> start() async {
    final response = await http.get(Uri.parse('$baseUrl/api/scavenger/start'));

    if (response.statusCode == 200) {
      return json.decode(response.body);
    } else {
      throw Exception('Failed to start scavenger');
    }
  }
}

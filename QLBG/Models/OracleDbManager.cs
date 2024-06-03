using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using QLBG.Models;

public class OracleDbManager
{
    private string connectionString;

    public OracleDbManager()
    {
        connectionString = ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString;
    }
    public User GetUser(string taiKhoan, string matKhau)
    {
        User user = null;
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = "SELECT * FROM USERS WHERE TaiKhoan = :TaiKhoan AND MatKhau = :MatKhau";
            using (OracleCommand cmd = new OracleCommand(query, conn))
            {
                cmd.Parameters.Add(new OracleParameter("TaiKhoan", taiKhoan));
                cmd.Parameters.Add(new OracleParameter("MatKhau", matKhau));

                conn.Open();
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new User
                        {
                            ID_KhachHang = Convert.ToInt32(reader["ID_KhachHang"]),
                            HoTen = reader["HoTen"].ToString(),
                            NgaySinh = Convert.ToDateTime(reader["NgaySinh"]),
                            DiaChi = reader["DiaChi"].ToString(),
                            GioiTinh = reader["GioiTinh"].ToString(),
                            Email = reader["Email"].ToString(),
                            SoDienThoai = reader["SoDienThoai"].ToString(),
                            TaiKhoan = reader["TaiKhoan"].ToString(),
                            MatKhau = reader["MatKhau"].ToString(),
                            Role = Convert.ToInt32(reader["Role"])
                        };
                    }
                }
            }
        }
        return user;
    }

    public string RegisterUser(User user)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            using (OracleCommand cmd = new OracleCommand("RegisterUser", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new OracleParameter("p_HoTen", OracleDbType.NVarchar2)).Value = user.HoTen;
                cmd.Parameters.Add(new OracleParameter("p_NgaySinh", OracleDbType.Date)).Value = user.NgaySinh;
                cmd.Parameters.Add(new OracleParameter("p_DiaChi", OracleDbType.NVarchar2)).Value = user.DiaChi;
                cmd.Parameters.Add(new OracleParameter("p_GioiTinh", OracleDbType.NVarchar2)).Value = user.GioiTinh;
                cmd.Parameters.Add(new OracleParameter("p_Email", OracleDbType.NVarchar2)).Value = user.Email;
                cmd.Parameters.Add(new OracleParameter("p_SoDienThoai", OracleDbType.NVarchar2)).Value = user.SoDienThoai;
                cmd.Parameters.Add(new OracleParameter("p_TaiKhoan", OracleDbType.NVarchar2)).Value = user.TaiKhoan;
                cmd.Parameters.Add(new OracleParameter("p_MatKhau", OracleDbType.NVarchar2)).Value = user.MatKhau;
                cmd.Parameters.Add(new OracleParameter("p_Role", OracleDbType.Int32)).Value = user.Role;

                var resultParam = new OracleParameter("p_Result", OracleDbType.Varchar2, 1000);
                resultParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(resultParam);

                conn.Open();
                cmd.ExecuteNonQuery();

                return resultParam.Value.ToString();
            }
        }
    }

    // Hàm lấy thông tin người dùng theo tài khoản
    public User GetUserByTaiKhoan(string taiKhoan)
    {
        User user = null;
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = "SELECT * FROM USERS WHERE TaiKhoan = :TaiKhoan";
            using (OracleCommand cmd = new OracleCommand(query, conn))
            {
                cmd.Parameters.Add(new OracleParameter("TaiKhoan", taiKhoan));

                conn.Open();
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new User
                        {
                            TaiKhoan = reader["TaiKhoan"].ToString(),
                            HoTen = reader["HoTen"].ToString(),
                            Email = reader["Email"].ToString(),
                            SoDienThoai = reader["SoDienThoai"].ToString(),
                            NgaySinh = reader["NgaySinh"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["NgaySinh"]),
                            DiaChi = reader["DiaChi"].ToString(),
                            GioiTinh = reader["GioiTinh"].ToString()
                        };
                    }
                }
            }
        }
        return user;
    }


    

    

    // Hàm cập nhật thông tin người dùng trong cơ sở dữ liệu


    public DataTable ExecuteQuery(string query)
    {
        DataTable dataTable = new DataTable();

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            using (OracleCommand cmd = new OracleCommand(query, conn))
            {
                conn.Open();
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    dataTable.Load(reader);
                }
            }
        }

        return dataTable;
    }

    public int ExecuteNonQuery(string commandText)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            using (OracleCommand cmd = new OracleCommand(commandText, conn))
            {
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }
    }

    public List<SanPham> GetSanPhams()
    {
        List<SanPham> sanPhams = new List<SanPham>();

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            string query = "SELECT * FROM SANPHAM";
            using (OracleCommand cmd = new OracleCommand(query, conn))
            {
                conn.Open();
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SanPham sanPham = new SanPham
                        {
                            ID_SanPham = Convert.ToInt32(reader["ID_SanPham"]),
                            TenSanPham = reader["TenSanPham"].ToString(),
                            ID_ThuongHieu = Convert.ToInt32(reader["ID_ThuongHieu"]),
                            ID_DanhMuc = Convert.ToInt32(reader["ID_DanhMuc"]),
                            ID_Anh = Convert.ToInt32(reader["ID_Anh"]),
                            ID_KichThuoc = Convert.ToInt32(reader["ID_KichThuoc"]),
                            ID_Mau = Convert.ToInt32(reader["ID_Mau"]),
                            Mota = reader["Mota"].ToString(),
                            DonViGia = Convert.ToDecimal(reader["DonViGia"]),
                            SoLuongTon = Convert.ToInt32(reader["SoLuongTon"])
                        };
                        sanPhams.Add(sanPham);
                    }
                }
            }
        }

        return sanPhams;
    }

    // Method to check if a user exists in the database

    public List<DanhMuc> GetAllDanhMucs()
    {
        List<DanhMuc> danhMucs = new List<DanhMuc>();

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT ID_DanhMuc, TenDanhMuc FROM DANHMUC";
            OracleCommand cmd = new OracleCommand(query, conn);

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    DanhMuc danhMuc = new DanhMuc
                    {
                        ID_DanhMuc = reader.GetInt32(0),
                        TenDanhMuc = reader.GetString(1)
                    };
                    danhMucs.Add(danhMuc);
                }
            }
        }

        return danhMucs;
    }

    public DanhMuc GetDanhMucById(int id)
    {
        DanhMuc danhMuc = null;

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT ID_DanhMuc, TenDanhMuc FROM DANHMUC WHERE ID_DanhMuc = :id";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(new OracleParameter("id", id));

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    danhMuc = new DanhMuc
                    {
                        ID_DanhMuc = reader.GetInt32(0),
                        TenDanhMuc = reader.GetString(1)
                    };
                }
            }
        }

        return danhMuc;
    }

    public bool DanhMucExists(int id)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT COUNT(*) FROM DANHMUC WHERE ID_DanhMuc = :id";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(new OracleParameter("id", id));

            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }
    }

    public void InsertDanhMuc(DanhMuc danhMuc)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "INSERT INTO DANHMUC (ID_DanhMuc, TenDanhMuc) VALUES (:id, :ten)";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(new OracleParameter("id", danhMuc.ID_DanhMuc));
            cmd.Parameters.Add(new OracleParameter("ten", danhMuc.TenDanhMuc));

            cmd.ExecuteNonQuery();
        }
    }

    public bool UpdateDanhMuc(DanhMuc danhMuc)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "UPDATE DANHMUC SET TenDanhMuc = :ten WHERE ID_DanhMuc = :id";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(new OracleParameter("id", danhMuc.ID_DanhMuc));
            cmd.Parameters.Add(new OracleParameter("ten", danhMuc.TenDanhMuc));

            int rowsUpdated = cmd.ExecuteNonQuery();
            return rowsUpdated > 0;
        }
    }

    public bool DeleteDanhMuc(int id)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string deleteSanPhamQuery = "DELETE FROM SANPHAM WHERE ID_DanhMuc = :id";
            string deleteCTDatHangQuery = "DELETE FROM CT_DATHANG WHERE ID_SanPham IN (SELECT ID_SanPham FROM SANPHAM WHERE ID_DanhMuc = :id)";
            string deleteDanhMucQuery = "DELETE FROM DANHMUC WHERE ID_DanhMuc = :id";

            using (OracleTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    OracleCommand cmd1 = new OracleCommand(deleteCTDatHangQuery, conn);
                    cmd1.Parameters.Add(new OracleParameter("id", id));
                    cmd1.ExecuteNonQuery();

                    OracleCommand cmd2 = new OracleCommand(deleteSanPhamQuery, conn);
                    cmd2.Parameters.Add(new OracleParameter("id", id));
                    cmd2.ExecuteNonQuery();

                    OracleCommand cmd3 = new OracleCommand(deleteDanhMucQuery, conn);
                    cmd3.Parameters.Add(new OracleParameter("id", id));
                    cmd3.ExecuteNonQuery();

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

    }

    public List<User> GetAllUsers()
    {
        List<User> users = new List<User>();

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT ID_KhachHang, HoTen, NgaySinh, DiaChi, GioiTinh, Email, SoDienThoai, TaiKhoan, MatKhau, Role FROM USERS";
            OracleCommand cmd = new OracleCommand(query, conn);

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    User user = new User
                    {
                        ID_KhachHang = reader.GetInt32(0),
                        HoTen = reader.GetString(1),
                        NgaySinh = reader.GetDateTime(2),
                        DiaChi = reader.GetString(3),
                        GioiTinh = reader.GetString(4),
                        Email = reader.GetString(5),
                        SoDienThoai = reader.GetString(6),
                        TaiKhoan = reader.GetString(7),
                        MatKhau = reader.GetString(8),
                        Role = reader.GetInt32(9)
                    };
                    users.Add(user);
                }
            }
        }

        return users;
    }

    public User GetUserById(int id)
    {
        User user = null;

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT ID_KhachHang, HoTen, NgaySinh, DiaChi, GioiTinh, Email, SoDienThoai, TaiKhoan, MatKhau, Role FROM USERS WHERE ID_KhachHang = :id";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(new OracleParameter("id", id));

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    user = new User
                    {
                        ID_KhachHang = reader.GetInt32(0),
                        HoTen = reader.GetString(1),
                        NgaySinh = reader.GetDateTime(2),
                        DiaChi = reader.GetString(3),
                        GioiTinh = reader.GetString(4),
                        Email = reader.GetString(5),
                        SoDienThoai = reader.GetString(6),
                        TaiKhoan = reader.GetString(7),
                        MatKhau = reader.GetString(8),
                        Role = reader.GetInt32(9)
                    };
                }
            }
        }

        return user;
    }

    public bool UpdateUser(User user)
    {
        try
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();
                string query = @"
            UPDATE USERS 
            SET HoTen = :HoTen, 
                NgaySinh = :NgaySinh, 
                DiaChi = :DiaChi, 
                GioiTinh = :GioiTinh, 
                Email = :Email, 
                SoDienThoai = :SoDienThoai, 
                Role = :Role 
            WHERE ID_KhachHang = :ID_KhachHang";

                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("HoTen", user.HoTen));
                    cmd.Parameters.Add(new OracleParameter("NgaySinh", user.NgaySinh.HasValue ? (object)user.NgaySinh.Value.Date : DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("DiaChi", user.DiaChi));
                    cmd.Parameters.Add(new OracleParameter("GioiTinh", user.GioiTinh));
                    cmd.Parameters.Add(new OracleParameter("Email", user.Email));
                    cmd.Parameters.Add(new OracleParameter("SoDienThoai", user.SoDienThoai));
                    cmd.Parameters.Add(new OracleParameter("Role", user.Role));
                    cmd.Parameters.Add(new OracleParameter("ID_KhachHang", user.ID_KhachHang));

                    int rowsUpdated = cmd.ExecuteNonQuery();
                    return rowsUpdated > 0;
                }
            }
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine("Error updating user: " + ex.Message);
            return false;
        }
    }




    public bool DeleteUser(int id)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "DELETE FROM USERS WHERE ID_KhachHang = :id";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(new OracleParameter("id", id));

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }


    public List<SanPham> GetAllSanPhams()
    {
        List<SanPham> sanPhams = new List<SanPham>();

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT * FROM SANPHAM";
            OracleCommand cmd = new OracleCommand(query, conn);

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    SanPham sanPham = new SanPham
                    {
                        ID_SanPham = Convert.ToInt32(reader["ID_SanPham"]),
                        TenSanPham = reader["TenSanPham"].ToString(),
                        ID_ThuongHieu = Convert.ToInt32(reader["ID_ThuongHieu"]),
                        ID_DanhMuc = Convert.ToInt32(reader["ID_DanhMuc"]),
                        ID_Anh = Convert.ToInt32(reader["ID_Anh"]),
                        ID_KichThuoc = Convert.ToInt32(reader["ID_KichThuoc"]),
                        ID_Mau = Convert.ToInt32(reader["ID_Mau"]),
                        Mota = reader["Mota"].ToString(),
                        DonViGia = Convert.ToDecimal(reader["DonViGia"]),
                        SoLuongTon = Convert.ToInt32(reader["SoLuongTon"])
                    };
                    sanPhams.Add(sanPham);
                }
            }
        }

        return sanPhams;
    }

    public SanPham GetSanPhamById(int id)
    {
        SanPham sanPham = null;

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT * FROM SANPHAM WHERE ID_SanPham = :id";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(new OracleParameter("id", id));

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    sanPham = new SanPham
                    {
                        ID_SanPham = Convert.ToInt32(reader["ID_SanPham"]),
                        TenSanPham = reader["TenSanPham"].ToString(),
                        ID_ThuongHieu = Convert.ToInt32(reader["ID_ThuongHieu"]),
                        ID_DanhMuc = Convert.ToInt32(reader["ID_DanhMuc"]),
                        ID_Anh = Convert.ToInt32(reader["ID_Anh"]),
                        ID_KichThuoc = Convert.ToInt32(reader["ID_KichThuoc"]),
                        ID_Mau = Convert.ToInt32(reader["ID_Mau"]),
                        Mota = reader["Mota"].ToString(),
                        DonViGia = Convert.ToDecimal(reader["DonViGia"]),
                        SoLuongTon = Convert.ToInt32(reader["SoLuongTon"])
                    };
                }
            }
        }

        return sanPham;
    }

    public void InsertSanPham(SanPham sanPham)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "INSERT INTO SANPHAM (ID_SanPham, TenSanPham, ID_ThuongHieu, ID_DanhMuc, ID_Anh, ID_KichThuoc, ID_Mau, Mota, DonViGia, SoLuongTon) VALUES (:id, :ten, :thuongHieu, :danhMuc, :anh, :kichThuoc, :mau, :mota, :gia, :soLuongTon)";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(new OracleParameter("id", sanPham.ID_SanPham));
            cmd.Parameters.Add(new OracleParameter("ten", sanPham.TenSanPham));
            cmd.Parameters.Add(new OracleParameter("thuongHieu", sanPham.ID_ThuongHieu));
            cmd.Parameters.Add(new OracleParameter("danhMuc", sanPham.ID_DanhMuc));
            cmd.Parameters.Add(new OracleParameter("anh", sanPham.ID_Anh));
            cmd.Parameters.Add(new OracleParameter("kichThuoc", sanPham.ID_KichThuoc));
            cmd.Parameters.Add(new OracleParameter("mau", sanPham.ID_Mau));
            cmd.Parameters.Add(new OracleParameter("mota", sanPham.Mota));
            cmd.Parameters.Add(new OracleParameter("gia", sanPham.DonViGia));
            cmd.Parameters.Add(new OracleParameter("soLuongTon", sanPham.SoLuongTon));

            cmd.ExecuteNonQuery();
        }
    }

    public bool UpdateSanPham(SanPham sanPham)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "UPDATE SANPHAM SET TenSanPham = :ten, ID_ThuongHieu = :thuongHieu, ID_DanhMuc = :danhMuc, ID_Anh = :anh, ID_KichThuoc = :kichThuoc, ID_Mau = :mau, Mota = :mota, DonViGia = :gia, SoLuongTon = :soLuongTon WHERE ID_SanPham = :id";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(new OracleParameter("id", sanPham.ID_SanPham));
            cmd.Parameters.Add(new OracleParameter("ten", sanPham.TenSanPham));
            cmd.Parameters.Add(new OracleParameter("thuongHieu", sanPham.ID_ThuongHieu));
            cmd.Parameters.Add(new OracleParameter("danhMuc", sanPham.ID_DanhMuc));
            cmd.Parameters.Add(new OracleParameter("anh", sanPham.ID_Anh));
            cmd.Parameters.Add(new OracleParameter("kichThuoc", sanPham.ID_KichThuoc));
            cmd.Parameters.Add(new OracleParameter("mau", sanPham.ID_Mau));
            cmd.Parameters.Add(new OracleParameter("mota", sanPham.Mota));
            cmd.Parameters.Add(new OracleParameter("gia", sanPham.DonViGia));
            cmd.Parameters.Add(new OracleParameter("soLuongTon", sanPham.SoLuongTon));

            int rowsUpdated = cmd.ExecuteNonQuery();
            return rowsUpdated > 0;
        }
    }

    public bool DeleteSanPham(int id)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string deleteCTDatHangQuery = "DELETE FROM CT_DATHANG WHERE ID_SanPham = :id";
            string deleteSanPhamQuery = "DELETE FROM SANPHAM WHERE ID_SanPham = :id";

            using (OracleTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    OracleCommand cmd1 = new OracleCommand(deleteCTDatHangQuery, conn);
                    cmd1.Parameters.Add(new OracleParameter("id", id));
                    cmd1.ExecuteNonQuery();

                    OracleCommand cmd2 = new OracleCommand(deleteSanPhamQuery, conn);
                    cmd2.Parameters.Add(new OracleParameter("id", id));
                    cmd2.ExecuteNonQuery();

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }
    }
        public HinhAnh GetAnhById(int id)
        {
            HinhAnh hinhAnh = null;

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM HINHANH WHERE ID_Anh = :id";
                OracleCommand cmd = new OracleCommand(query, conn);
                cmd.Parameters.Add(new OracleParameter("id", id));

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        hinhAnh = new HinhAnh
                        {
                            ID_Anh = Convert.ToInt32(reader["ID_Anh"]),
                            AnhChinh = reader["AnhChinh"].ToString(),
                            Anh1 = reader["Anh1"].ToString(),
                            Anh2 = reader["Anh2"].ToString()
                        };
                    }
                }
            }

            return hinhAnh;
        }

    public bool SanPhamExists(int id)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT COUNT(*) FROM SANPHAM WHERE ID_SanPham = :id";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(new OracleParameter("id", id));

            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }
    }

    public List<CTDatHang> GetCTDathangBySanPhamId(int id)
    {
        List<CTDatHang> cTDathangs = new List<CTDatHang>();

        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT * FROM CT_DATHANG WHERE ID_SanPham = :id";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(new OracleParameter("id", id));

            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    CTDatHang cTDathang = new CTDatHang
                    {
                        ID_CTDatHang = Convert.ToInt32(reader["ID_CT_DatHang"]),
                        ID_DatHang = Convert.ToInt32(reader["ID_DatHang"]),
                        ID_SanPham = Convert.ToInt32(reader["ID_SanPham"]),
                        SoLuong = Convert.ToInt32(reader["SoLuong"]),
                        DonViGia = Convert.ToDecimal(reader["DonGia"])
                    };
                    cTDathangs.Add(cTDathang);
                }
            }
        }

        return cTDathangs;
    }

    public bool DeleteAllCTDathang(int id)
    {
        using (OracleConnection conn = new OracleConnection(connectionString))
        {
            conn.Open();
            string query = "DELETE FROM CT_DATHANG WHERE ID_SanPham = :id";
            OracleCommand cmd = new OracleCommand(query, conn);
            cmd.Parameters.Add(new OracleParameter("id", id));

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }



}


